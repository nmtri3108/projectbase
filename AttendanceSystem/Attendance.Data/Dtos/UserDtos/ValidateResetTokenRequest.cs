using System.ComponentModel.DataAnnotations;

namespace Attendance.Data.Dtos.UserDtos
{
    public class ValidateResetTokenRequest
    {
        [Required]
        public string Token { get; set; }
    }
}
