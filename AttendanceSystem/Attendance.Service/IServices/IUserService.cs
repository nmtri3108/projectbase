using Attendance.Common.Constants;
using Attendance.Data.Dtos.UserDtos;
using Microsoft.AspNetCore.Http;

namespace Attendance.Service.IServices
{
    public interface IUserService
    {
        Task<AuthenticateResponse> Authenticate(AuthenticateRequest model, string ipAddress);
        Task<AuthenticateResponse> RefreshToken(string token, string ipAddress);
        Task RevokeToken(string token, string ipAddress);
        Task Register(RegisterRequest model, string origin);
        Task VerifyEmail(string token);
        Task ForgotPassword(ForgotPasswordRequest model, string origin);
        Task ValidateResetToken(ValidateResetTokenRequest model);
        Task ResetPassword(ResetPasswordRequest model);
        Task<IEnumerable<AccountResponse>> GetAll();
        Task<AccountResponse> GetById(string id);
        Task<AccountResponse> Create(CreateRequest model);
        Task<AccountResponse> Update(string id, UpdateRequest model, StringEnum.Roles role);
        Task<AccountResponse> UpdateSelf(string id, UpdateSelfRequest model);
        Task<bool> AdminCheck();
        Task Delete(string id);
        List<string> GetFields();
        Task<List<Dictionary<string, string>>> ReadExcel(IFormFile file);
        Task<IList<AccountResponse>> ImportExcel(IFormFile file, string mapping);
        Task<IEnumerable<AccountResponse>> GetByDempartment(StringEnum.Departments department);
    }
}
