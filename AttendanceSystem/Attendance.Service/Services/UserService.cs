using Attendance.Common.Constants;
using Attendance.Common.Exceptions;
using Attendance.Common.Utility;
using Attendance.Data.Context;
using Attendance.Data.Dtos.UserDtos;
using Attendance.Data.Models.UserModels;
using Attendance.Service.IServices;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Security.Cryptography;
using OfficeOpenXml;
using System.Data;

namespace Attendance.Service.Services
{
    public class UserService : IUserService
    {
        private readonly ApplicationDbContext db;
        private readonly IJwtUtils jwtUtils;
        private readonly IMapper mapper;
        private readonly IEmailService emailService;

        public UserService(ApplicationDbContext db, IJwtUtils jwtUtils, IMapper mapper, IEmailService emailService)
        {
            this.db = db;
            this.jwtUtils = jwtUtils;
            this.mapper = mapper;
            this.emailService = emailService;
        }

        public async Task<AuthenticateResponse> Authenticate(AuthenticateRequest model, string ipAddress)
        {
            var account = await db.Accounts.SingleOrDefaultAsync(x => x.Email == model.Email);

            // validate
            if (account == null || !account.IsVerified ||
                !BCrypt.Net.BCrypt.Verify(model.Password, account.PasswordHash))
                throw new AppException("Email or password is incorrect");

            // authentication successful so generate jwt and refresh tokens
            var jwtToken = jwtUtils.GenerateJwtToken(account);
            var refreshToken = await jwtUtils.GenerateRefreshToken(ipAddress);
            account.RefreshTokens.Add(refreshToken);

            // remove old refresh tokens from account
            RemoveOldRefreshTokens(account);

            // save changes to db
            await db.SaveChangesAsync();

            var response = mapper.Map<AuthenticateResponse>(account);
            response.JwtToken = jwtToken;
            response.RefreshToken = refreshToken.Token;

            return response;
        }

        public async Task<AuthenticateResponse> RefreshToken(string token, string ipAddress)
        {
            var account = await GetAccountByRefreshToken(token);
            var refreshToken = account.RefreshTokens.Single(x => x.Token == token);

            if (refreshToken.IsRevoked)
            {
                // revoke all descendant tokens in case this token has been compromised
                await RevokeDescendantRefreshTokens(refreshToken, account, ipAddress,
                    $"Attempted reuse of revoked ancestor token: {token}");

                await db.SaveChangesAsync();
            }

            if (!refreshToken.IsActive)
                throw new AppException("Invalid token");

            // replace old refresh token with a new one (rotate token)
            var newRefreshToken = await RotateRefreshToken(refreshToken, ipAddress);
            account.RefreshTokens.Add(newRefreshToken);


            // remove old refresh tokens from account
            RemoveOldRefreshTokens(account);

            // save changes to db
            await db.SaveChangesAsync();

            // generate new jwt
            var jwtToken = jwtUtils.GenerateJwtToken(account);

            // return data in authenticate response object
            var response = mapper.Map<AuthenticateResponse>(account);
            response.JwtToken = jwtToken;
            response.RefreshToken = newRefreshToken.Token;
            return response;
        }

        public async Task Register(RegisterRequest model, string origin)
        {
            // validate
            if (await db.Accounts.AnyAsync(x => x.Email == model.Email))
            {
                // send already registered error in email to prevent account enumeration
                await SendAlreadyRegisteredEmail(model.Email, origin);
                return;
            }

            // map model to new account object
            var account = mapper.Map<Account>(model);

            // first registered account is an admin
            account.Role = StringEnum.Roles.GeneralEmployee;
            account.Created = DateTime.UtcNow;
            account.VerificationToken = await GenerateVerificationToken();

            // hash password
            account.PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.Password);

            // prefix for user is dev
            account.Extension = new DeveloperExtension()
            {
                Band = StringEnum.Bands.A1,
                TechDirection = "Probation"
            };

            // save account
            await db.Accounts.AddAsync(account);
            await db.SaveChangesAsync();

            // send email
            await SendVerificationEmail(account, origin);
        }

        public async Task<AccountResponse> Create(CreateRequest model)
        {
            // validate
            if (await db.Accounts.AnyAsync(x => x.Email == model.Email))
                throw new AppException($"Email '{model.Email}' is already registered");

            // map model to new account object
            var account = new Account();
            account.Email = model.Email;
            account.Sex = model.Sex;
            account.PhoneNumber = model.PhoneNumber;
            account.IsIntern = model.IsIntern;
            account.Department = model.Department;
            account.Role = model.Role;
            account.FirstName = model.FirstName;
            account.LastName = model.LastName;
            account.Title = model.Title;
            account.AcceptTerms = true;
            account.Created = DateTime.UtcNow;
            account.Verified = DateTime.UtcNow;

            // hash password
            account.PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.Password);

            // match extension
            var serializedExtension = model.Extension != null ? model.Extension.ToString() : null;
            if (serializedExtension != null)
            {
                switch (account.Type)
                {
                    case StringEnum.EmployeeTypes.Dev:
                        account.Extension = JsonConvert.DeserializeObject<DeveloperExtension>(serializedExtension, new JsonSerializerSettings
                        {
                            TypeNameHandling = TypeNameHandling.Auto,
                        });
                        break;
                    case StringEnum.EmployeeTypes.QA:
                        account.Extension = JsonConvert.DeserializeObject<QualityAssuranceExtension>(serializedExtension, new JsonSerializerSettings
                        {
                            TypeNameHandling = TypeNameHandling.Auto,
                        });
                        break;
                    case StringEnum.EmployeeTypes.Manager:
                        account.Extension = JsonConvert.DeserializeObject<ManagerExtension>(serializedExtension, new JsonSerializerSettings
                        {
                            TypeNameHandling = TypeNameHandling.Auto
                        });
                        break;
                    default:
                        throw new ArgumentException("Invalid role type");
                }
            }

            // save account
            await db.Accounts.AddAsync(account);
            await db.SaveChangesAsync();

            return mapper.Map<AccountResponse>(account);
        }

        public async Task Delete(string id)
        {
            var account = await GetAccount(id);
            db.Accounts.Remove(account);
            db.SaveChanges();
        }

        public async Task ForgotPassword(ForgotPasswordRequest model, string origin)
        {
            var account = await db.Accounts.SingleOrDefaultAsync(x => x.Email == model.Email);

            // always return ok response to prevent email enumeration
            if (account == null) return;

            // create reset token that expires after 1 day
            account.ResetToken = await GenerateResetToken();
            account.ResetTokenExpires = DateTime.UtcNow.AddDays(1);

            await db.SaveChangesAsync();

            // send email
            await SendPasswordResetEmail(account, origin);
        }

        public async Task<IEnumerable<AccountResponse>> GetAll()
        {
            var accounts = await db.Accounts.ToListAsync();
            return mapper.Map<IList<AccountResponse>>(accounts);
        }

        public async Task<AccountResponse> GetById(string id)
        {
            var account = await GetAccount(id);
            return mapper.Map<AccountResponse>(account);
        }

        public async Task<IEnumerable<AccountResponse>> GetByDempartment(StringEnum.Departments department)
        {
            var account = await db.Accounts.Where(_=>_.Department == department).ToListAsync();
            if (account == null) throw new KeyNotFoundException("Account not found");

            return mapper.Map<IList<AccountResponse>>(account);
        }

        public async Task VerifyEmail(string token)
        {
            var account = await db.Accounts.SingleOrDefaultAsync(x => x.VerificationToken == token);

            if (account == null)
                throw new AppException("Verification failed");

            account.Verified = DateTime.UtcNow;
            account.VerificationToken = null;

            await db.SaveChangesAsync();
        }


        public async Task ResetPassword(ResetPasswordRequest model)
        {
            var account = await GetAccountByResetToken(model.Token);

            // update password and remove reset token
            account.PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.Password);
            account.PasswordReset = DateTime.UtcNow;
            account.ResetToken = null;
            account.ResetTokenExpires = null;

            await db.SaveChangesAsync();
        }

        public async Task RevokeToken(string token, string ipAddress)
        {
            var account = await GetAccountByRefreshToken(token);
            var refreshToken = account.RefreshTokens.Single(x => x.Token == token);

            if (!refreshToken.IsActive)
                throw new AppException("Invalid token");

            // revoke token and save
            RevokeRefreshToken(refreshToken, ipAddress, "Revoked without replacement");

            await db.SaveChangesAsync();
        }

        public async Task<AccountResponse> Update(string id, UpdateRequest model, StringEnum.Roles role)
        {
            var account = await GetAccount(id);

            // validate
            if (account.Email != model.Email && await db.Accounts.AnyAsync(x => x.Email == model.Email))
                throw new AppException($"Email '{model.Email}' is already registered");

            // hash password if it was entered
            if (!string.IsNullOrEmpty(model.Password))
                account.PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.Password);

            // copy model to account and save
            var currentRole = account.Role;
            mapper.Map(model, account);
            account.Updated = DateTime.UtcNow;

            if (role != StringEnum.Roles.Administrator)
            {
                account.Role = currentRole;
            }

            // only admins can update role
            if (role == StringEnum.Roles.Administrator)
            {
                var serializedExtension = model.Extension != null ? model.Extension.ToString() : null;
                if (serializedExtension != null)
                {
                    switch (account.Type)
                    {
                        case StringEnum.EmployeeTypes.Dev:
                            account.Extension = JsonConvert.DeserializeObject<DeveloperExtension>(serializedExtension, new JsonSerializerSettings
                            {
                                TypeNameHandling = TypeNameHandling.Auto,
                            });
                            break;
                        case StringEnum.EmployeeTypes.QA:
                            account.Extension = JsonConvert.DeserializeObject<QualityAssuranceExtension>(serializedExtension, new JsonSerializerSettings
                            {
                                TypeNameHandling = TypeNameHandling.Auto,
                            });
                            break;
                        case StringEnum.EmployeeTypes.Manager:
                            account.Extension = JsonConvert.DeserializeObject<ManagerExtension>(serializedExtension, new JsonSerializerSettings
                            {
                                TypeNameHandling = TypeNameHandling.Auto
                            });
                            break;
                        default:
                            throw new ArgumentException("Invalid role type");
                    }
                }
            }

            await db.SaveChangesAsync();

            return mapper.Map<AccountResponse>(account);
        }

        public async Task<AccountResponse> UpdateSelf(string id, UpdateSelfRequest model)
        {
            var account = await GetAccount(id);

            // hash password if it was entered
            if (!string.IsNullOrEmpty(model.Password))
                account.PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.Password);

            // copy model to account and save
            mapper.Map(model, account);
            account.Updated = DateTime.UtcNow;

            await db.SaveChangesAsync();

            return mapper.Map<AccountResponse>(account);
        }

        public async Task ValidateResetToken(ValidateResetTokenRequest model)
        {
            await GetAccountByResetToken(model.Token);
        }

        public async Task<bool> AdminCheck()
        {
            return await db.Accounts.AnyAsync(_ => _.Role == StringEnum.Roles.Administrator);
        }

        public List<string> GetFields()
        {
            return new List<string>()
            {
                "Title", "FirstName", "LastName", "Email","Role", "Password", "ConfirmPassword", "Type", "PhoneNumber",
                "IsIntern", "Department", "Sex", "Extension", 
            };
        }

        public async Task<List<Dictionary<string, string>>> ReadExcel(IFormFile file)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using (var package = new ExcelPackage(file.OpenReadStream()))
            {
                var worksheet = package.Workbook.Worksheets[0];

                // Extract data into a list of dictionaries
                var data = new List<Dictionary<string, string>>();
                for (var rowNumber = 1; rowNumber <= worksheet.Dimension.End.Row; rowNumber++)
                {
                    var row = worksheet.Cells[rowNumber, 1, rowNumber, worksheet.Dimension.End.Column];
                    var rowData = new Dictionary<string, string>();
                    foreach (var cell in row)
                    {
                        rowData[cell.Start.Column.ToString()] = cell.Text;
                    }

                    data.Add(rowData);
                }

                return data;
            }
        }

        public async Task<IList<AccountResponse>> ImportExcel(IFormFile file, string mapping)
        {
            // Load the Excel file using EPPlus
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using (var package = new ExcelPackage(file.OpenReadStream()))
            {
                var worksheet = package.Workbook.Worksheets[0];

                // Extract data into a DataTable
                DataTable dt = new DataTable();
                foreach (var firstRowCell in worksheet.Cells[1, 1, 1, worksheet.Dimension.End.Column])
                {
                    dt.Columns.Add(firstRowCell.Text);
                }

                for (var rowNumber = 2; rowNumber <= worksheet.Dimension.End.Row; rowNumber++)
                {
                    var row = worksheet.Cells[rowNumber, 1, rowNumber, worksheet.Dimension.End.Column];
                    var newRow = dt.NewRow();
                    var count = 0;
                    foreach (var cell in row)
                    {
                        newRow[count] = cell.Text;
                        count++;
                    }

                    dt.Rows.Add(newRow);
                }

                // Deserialize the mapping string into a C# object
                var userMapping =
                    Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, string>>(mapping);

                // Map Excel columns to appropriate fields
                var mappedData = new List<CreateRequest>();
                foreach (DataRow row in dt.Rows)
                {
                    var item = new CreateRequest();

                    foreach (var mappingEntry in userMapping)
                    {
                        var excelColumn = mappingEntry.Value;
                        var dataField = mappingEntry.Key;

                        // Map Excel data to the corresponding field
                        if (dt.Columns.Contains(excelColumn))
                        {
                            var value = row[excelColumn].ToString();

                            var propertyInfo = typeof(CreateRequest).GetProperty(dataField);
                           
                            if (propertyInfo?.PropertyType == typeof(bool) || propertyInfo?.PropertyType == typeof(bool?))
                            {
                                bool boolValue;
                                if (bool.TryParse(value, out boolValue))
                                {
                                    // Successfully converted to int
                                    propertyInfo.SetValue(item, boolValue);
                                }
                                else
                                {
                                    propertyInfo.SetValue(item, true);
                                }
                            }
                            else if (propertyInfo?.PropertyType == typeof(StringEnum.Roles) || propertyInfo?.PropertyType == typeof(StringEnum.Roles?))
                            {
                                StringEnum.Roles role;
                                if (StringEnum.Roles.TryParse(value, out role))
                                {
                                    // Successfully converted to int
                                    propertyInfo.SetValue(item, role);
                                }
                                else
                                {
                                    propertyInfo.SetValue(item, StringEnum.Roles.GeneralEmployee);
                                }
                            }
                            else if (propertyInfo?.PropertyType == typeof(StringEnum.EmployeeTypes) || propertyInfo?.PropertyType == typeof(StringEnum.EmployeeTypes?))
                            {
                                StringEnum.EmployeeTypes type;
                                if (StringEnum.EmployeeTypes.TryParse(value, out type))
                                {
                                    // Successfully converted to int
                                    propertyInfo.SetValue(item, type);
                                }
                                else
                                {
                                    propertyInfo.SetValue(item, StringEnum.EmployeeTypes.Dev);
                                }
                            }
                            else if (propertyInfo?.PropertyType == typeof(StringEnum.Sex) || propertyInfo?.PropertyType == typeof(StringEnum.Sex?))
                            {
                                StringEnum.Sex sex;
                                if (StringEnum.Sex.TryParse(value, out sex))
                                {
                                    // Successfully converted to int
                                    propertyInfo.SetValue(item, sex);
                                }
                                else
                                {
                                    propertyInfo.SetValue(item, StringEnum.Sex.Male);
                                }
                            }
                            else if (propertyInfo?.PropertyType == typeof(StringEnum.Departments) || propertyInfo?.PropertyType == typeof(StringEnum.Departments?))
                            {
                                StringEnum.Departments department;
                                if (StringEnum.Departments.TryParse(value, out department))
                                {
                                    // Successfully converted to int
                                    propertyInfo.SetValue(item, department);
                                }
                                else
                                {
                                    propertyInfo.SetValue(item, StringEnum.Departments.Dev);
                                }
                            }
                            else if (propertyInfo?.Name == nameof(CreateRequest.Extension))
                            {
                                // match extension
                                if (value != null)
                                {
                                    switch (item.Type)
                                    {
                                        case StringEnum.EmployeeTypes.Dev:
                                            propertyInfo?.SetValue(item, JsonConvert.DeserializeObject<DeveloperExtension>(value, new JsonSerializerSettings
                                            {
                                                TypeNameHandling = TypeNameHandling.Auto,
                                            }));
                                            break;
                                        case StringEnum.EmployeeTypes.QA:
                                            propertyInfo?.SetValue(item, JsonConvert.DeserializeObject<QualityAssuranceExtension>(value, new JsonSerializerSettings
                                            {
                                                TypeNameHandling = TypeNameHandling.Auto,
                                            }));
                                            break;
                                        case StringEnum.EmployeeTypes.Manager:
                                            propertyInfo?.SetValue(item, JsonConvert.DeserializeObject<ManagerExtension>(value, new JsonSerializerSettings
                                            {
                                                TypeNameHandling = TypeNameHandling.Auto
                                            }));
                                            break;
                                        default:
                                            throw new ArgumentException("Invalid role type");
                                    }

                                }
                            }
                            else
                            {
                                propertyInfo?.SetValue(item, value);
                            }
                        }
                    }

                    mappedData.Add(item);
                }

                var result = await ImportUsers(mappedData);

                return result;
            }
        }

        #region Helpers
        private async Task<IList<AccountResponse>> ImportUsers(List<CreateRequest> input)
        {
            var accounts = await db.Accounts.ToListAsync();
            var validData = new List<Account>();

            // Process and validate the data as needed and save
            foreach (var model in input)
            {
                if (!CheckValidData(model, validData, accounts))
                    continue;

                var account = new Account();
                account.Email = model.Email;
                account.Sex = model.Sex;
                account.PhoneNumber = model.PhoneNumber;
                account.IsIntern = model.IsIntern;
                account.Department = model.Department;
                account.Role = model.Role;
                account.FirstName = model.FirstName;
                account.LastName = model.LastName;
                account.Title = model.Title;
                account.Type = model.Type;
                account.Extension = (EmployeeExtension?) model.Extension;
                account.AcceptTerms = true;
                account.Created = DateTime.UtcNow;
                account.Verified = DateTime.UtcNow;

                // hash password
                account.PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.Password);

                validData.Add(account);
            }

            await db.Accounts.AddRangeAsync(validData);
            await db.SaveChangesAsync();

            return mapper.Map<List<AccountResponse>>(validData);
        }
        private bool CheckValidData(CreateRequest account, List<Account> validData, List<Account> currentAccounts)
        {
            var basicValidation = !string.IsNullOrEmpty(account.Email)
                                  && !string.IsNullOrEmpty(account.Password)
                                  && !string.IsNullOrEmpty(account.ConfirmPassword)
                                  && account.Password == account.ConfirmPassword
                                  && !string.IsNullOrEmpty(account.PhoneNumber)
                                  && !string.IsNullOrEmpty(account.Title)
                                  && !string.IsNullOrEmpty(account.FirstName)
                                  && !string.IsNullOrEmpty(account.LastName)
                                  && account.Role != null
                                  && account.Sex != null
                                  && account.Department != null;

            var isEmailInUsed = validData.Any(_ => _.Email.Trim().ToLower() == account.Email.Trim().ToLower())
                                || currentAccounts.Any(_ => _.Email.Trim().ToLower() == account.Email.Trim().ToLower());


            return basicValidation && !isEmailInUsed;
        }

        private async Task<Account> GetAccount(string id)
        {
            var account = await db.Accounts.FindAsync(id);
            if (account == null) throw new KeyNotFoundException("Account not found");
            return account;
        }

        private async Task<string> GenerateResetToken()
        {
            // token is a cryptographically strong random sequence of values
            var token = Convert.ToHexString(RandomNumberGenerator.GetBytes(64));

            // ensure token is unique by checking against db
            var isTokenExisted = await db.Accounts.AnyAsync(x => x.ResetToken == token);
            if (isTokenExisted)
                return await GenerateResetToken();

            return token;
        }

        private void RemoveOldRefreshTokens(Account account)
        {
            account.RefreshTokens.RemoveAll(x =>
                !x.IsActive &&
                x.Created.AddDays(AppSettings.RefreshTokenTTL) <= DateTime.UtcNow);
        }

        private async Task RevokeDescendantRefreshTokens(RefreshToken refreshToken, Account account, string ipAddress,
            string reason)
        {
            // recursively traverse the refresh token chain and ensure all descendants are revoked
            if (!string.IsNullOrEmpty(refreshToken.ReplacedByToken))
            {
                var childToken = account.RefreshTokens.SingleOrDefault(x => x.Token == refreshToken.ReplacedByToken);
                if (childToken.IsActive)
                    RevokeRefreshToken(childToken, ipAddress, reason);
                else
                    await RevokeDescendantRefreshTokens(childToken, account, ipAddress, reason);
            }
        }

        private void RevokeRefreshToken(RefreshToken token, string ipAddress, string reason = null,
            string replacedByToken = null)
        {
            token.Revoked = DateTime.UtcNow;
            token.RevokedByIp = ipAddress;
            token.ReasonRevoked = reason;
            token.ReplacedByToken = replacedByToken;
        }

        private async Task<RefreshToken> RotateRefreshToken(RefreshToken refreshToken, string ipAddress)
        {
            var newRefreshToken = await jwtUtils.GenerateRefreshToken(ipAddress);
            RevokeRefreshToken(refreshToken, ipAddress, "Replaced by new token", newRefreshToken.Token);
            return newRefreshToken;
        }

        private async Task<string> GenerateVerificationToken()
        {
            // token is a cryptographically strong random sequence of values
            var token = Convert.ToHexString(RandomNumberGenerator.GetBytes(64));

            // ensure token is unique by checking against db
            var isTokenExisted = await db.Accounts.AnyAsync(x => x.VerificationToken == token);
            if (isTokenExisted)
                return await GenerateVerificationToken();

            return token;
        }

        private async Task<Account> GetAccountByResetToken(string token)
        {
            var account = await db.Accounts.SingleOrDefaultAsync(x =>
                x.ResetToken == token && x.ResetTokenExpires > DateTime.UtcNow);
            if (account == null) throw new AppException("Invalid token");
            return account;
        }

        private async Task<Account> GetAccountByRefreshToken(string token)
        {
            var account = await db.Accounts.SingleOrDefaultAsync(u => u.RefreshTokens.Any(t => t.Token == token));
            if (account == null) throw new AppException("Invalid token");
            return account;
        }


        private async Task SendAlreadyRegisteredEmail(string email, string origin)
        {
            string message = await File.ReadAllTextAsync(FileHelper.GetEmailTemplateDirectory("AlreadyRegisteredEmail.html"));
            message = message.Replace("[[name]]", email);
            if (!string.IsNullOrEmpty(origin))
            {
                var returnUrl = $"{origin}/account/forgot-password";
                message = message.Replace("[[link]]", returnUrl);
            }
            else
                message =
                    "<p>If you don't know your password you can reset it via the <code>/accounts/forgot-password</code> api route.</p>";

            await emailService.Send(
                to: email,
                subject: "Sign-up Verification API - Email Already Registered",
                html: message
            );
        }

        private async Task SendVerificationEmail(Account account, string origin)
        {
            string message = await File.ReadAllTextAsync(FileHelper.GetEmailTemplateDirectory("VerifyEmail.html"));
            message = message.Replace("[[name]]", account.Email);
            if (!string.IsNullOrEmpty(origin))
            {
                // origin exists if request sent from browser single page app (e.g. Angular or React)
                // so send link to verify via single page app
                var verifyUrl = $"{origin}/account/verify-email?token={account.VerificationToken}";
                message = message.Replace("[[link]]", verifyUrl);
            }
            else
            {
                // origin missing if request sent directly to api (e.g. from Postman)
                // so send instructions to verify directly with api
                message =
                    $@"<p>Please use the below token to verify your email address with the <code>/accounts/verify-email</code> api route:</p>
                            <p><code>{account.VerificationToken}</code></p>";
            }

            await emailService.Send(
                to: account.Email,
                subject: "Sign-up Verification API - Verify Email",
                html: message
            );
        }

        private async Task SendPasswordResetEmail(Account account, string origin)
        {
            string message = await File.ReadAllTextAsync(FileHelper.GetEmailTemplateDirectory("PasswordReset.html"));
            message = message.Replace("[[name]]", account.Email);

            if (!string.IsNullOrEmpty(origin))
            {
                var resetUrl = $"{origin}/account/reset-password?token={account.ResetToken}";
                message = message.Replace("[[link]]", resetUrl);
            }
            else
            {
                message =
                    $@"<p>Please use the below token to reset your password with the <code>/accounts/reset-password</code> api route:</p>
                            <p><code>{account.ResetToken}</code></p>";
            }

            await emailService.Send(
                to: account.Email,
                subject: "Sign-up Verification API - Reset Password",
                html: message
            );
        }

        #endregion
    }
}
