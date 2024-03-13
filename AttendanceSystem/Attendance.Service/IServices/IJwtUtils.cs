using Attendance.Data.Models.UserModels;

namespace Attendance.Service.IServices
{
    public interface IJwtUtils
    {
        public string GenerateJwtToken(Account account);
        public string? ValidateJwtToken(string token);
        public Task<RefreshToken> GenerateRefreshToken(string ipAddress);
    }
}
