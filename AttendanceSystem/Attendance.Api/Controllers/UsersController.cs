using Attendance.Api.CustomAttributes;
using Attendance.Common.Constants;
using Attendance.Data.Dtos.UserDtos;
using Attendance.Service.IServices;
using Attendance.Service.Services;
using Microsoft.AspNetCore.Mvc;

namespace Attendance.Api.Controllers
{
    [Authorize]
    public class UsersController : BaseController
    {
        private readonly IUserService userService;

        public UsersController(IUserService userService)
        {
            this.userService = userService;
        }

        [AllowAnonymous]
        [HttpPost("authenticate")]
        public async Task<ActionResult<AuthenticateResponse>> Authenticate(AuthenticateRequest model)
        {
            var response = await userService.Authenticate(model, ipAddress());
            setTokenCookie(response.RefreshToken);
            return Ok(response);
        }

        [AllowAnonymous]
        [HttpPost("refresh-token")]
        public async Task<ActionResult<AuthenticateResponse>> RefreshToken()
        {
            var refreshToken = Request.Cookies["refreshToken"];
            var response = await userService.RefreshToken(refreshToken, ipAddress());
            setTokenCookie(response.RefreshToken);
            return Ok(response);
        }

        [HttpPost("revoke-token")]
        public async Task<IActionResult> RevokeToken(RevokeTokenRequest model)
        {
            // accept token from request body or cookie
            var token = model.Token ?? Request.Cookies["refreshToken"];

            if (string.IsNullOrEmpty(token))
                return BadRequest(new { message = "Token is required" });

            // users can revoke their own tokens and admins can revoke any tokens
            if (!Account.OwnsToken(token) && Account.Role != StringEnum.Roles.Administrator)
                return Unauthorized(new { message = "Unauthorized" });

            await userService.RevokeToken(token, ipAddress());
            return Ok(new { message = "Token revoked" });
        }

        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterRequest model)
        {
            await userService.Register(model, Request.Headers["origin"]);
            return Ok(new
            { message = "Registration successful, please check your email for verification instructions" });
        }

        [AllowAnonymous]
        [HttpPost("verify-email")]
        public async Task<IActionResult> VerifyEmail(VerifyEmailRequest model)
        {
            await userService.VerifyEmail(model.Token);
            return Ok(new { message = "Verification successful, you can now login" });
        }

        [AllowAnonymous]
        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordRequest model)
        {
            await userService.ForgotPassword(model, Request.Headers["origin"]);
            return Ok(new { message = "Please check your email for password reset instructions" });
        }

        [AllowAnonymous]
        [HttpPost("validate-reset-token")]
        public async Task<IActionResult> ValidateResetToken(ValidateResetTokenRequest model)
        {
            await userService.ValidateResetToken(model);
            return Ok(new { message = "Token is valid" });
        }

        [AllowAnonymous]
        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword(ResetPasswordRequest model)
        {
           await userService.ResetPassword(model);
            return Ok(new { message = "Password reset successful, you can now login" });
        }

        [Authorize(StringEnum.Roles.Administrator)]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AccountResponse>>> GetAll()
        {
            var accounts = await userService.GetAll();
            return Ok(accounts);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<AccountResponse>> GetById(string id)
        {
            // users can get their own account and admin can get any account
            if (id != Account.Id && Account.Role != StringEnum.Roles.Administrator)
                return Unauthorized(new { message = "Unauthorized" });

            var account = await userService.GetById(id);
            return Ok(account);
        }

        [Authorize(StringEnum.Roles.Administrator)]
        [HttpPost]
        public async Task<ActionResult<AccountResponse>> Create(CreateRequest model)
        {
            var account = await userService.Create(model);
            return Ok(account);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<AccountResponse>> Update(string id, UpdateRequest model)
        {
            // users can update their own account and admins can update any account
            if (id != Account.Id && Account.Role != StringEnum.Roles.Administrator)
                return Unauthorized(new { message = "Unauthorized" });

            var account = await userService.Update(id, model, Account.Role);
            return Ok(account);
        }

        [HttpPut("update-self/{id}")]
        public async Task<ActionResult<AccountResponse>> UpdateSelf(string id, UpdateSelfRequest model)
        {
            var account = await userService.UpdateSelf(id, model);
            return Ok(account);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            // users can delete their own account and admins can delete any account
            if (id != Account.Id && Account.Role != StringEnum.Roles.Administrator)
                return Unauthorized(new { message = "Unauthorized" });

            await userService.Delete(id);
            return Ok(new { message = "Account deleted successfully" });
        }

        [HttpGet("object-fields")]
        public IActionResult GetObjectFields()
        {
            List<string> availableFields = userService.GetFields();
            return Ok(availableFields);
        }

        [HttpPost("read-excel")]
        public async Task<IActionResult> ReadExcelFile([FromForm] UploadExcelModel model)
        {
            try
            {
                if (model.file == null || model.file.Length == 0)
                {
                    return BadRequest("No file uploaded.");
                }

                var result = await userService.ReadExcel(model.file);
                return Ok(result);
            }
            catch (Exception ex)
            {
                // Handle exceptions and return an error response
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [Authorize(StringEnum.Roles.Administrator)]
        [HttpPost("import")]
        public async Task<ActionResult<IEnumerable<AccountResponse>>> Import([FromForm] ImportExcelModel model)
        {
            try
            {
                if (model.file == null || model.file.Length == 0)
                {
                    return BadRequest("No file uploaded.");
                }

                var result = await userService.ImportExcel(model.file, model.mapping);
                return Ok(result);
            }
            catch (Exception ex)
            {
                // Handle exceptions and return an error response
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        #region Helpers
        private void setTokenCookie(string token)
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Expires = DateTime.UtcNow.AddDays(7)
            };
            Response.Cookies.Append("refreshToken", token, cookieOptions);
        }

        private string ipAddress()
        {
            if (Request.Headers.ContainsKey("X-Forwarded-For"))
                return Request.Headers["X-Forwarded-For"];
            else
                return HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
        }
        #endregion
    }
}
