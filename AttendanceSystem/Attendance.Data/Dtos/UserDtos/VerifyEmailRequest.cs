using System.ComponentModel.DataAnnotations;

namespace Attendance.Data.Dtos.UserDtos
{
    public class VerifyEmailRequest
    {
        [Required]
        public string Token { get; set; }
    }
}
