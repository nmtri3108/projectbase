using System.ComponentModel.DataAnnotations;

namespace Attendance.Data.Dtos.UserDtos
{
    public class ForgotPasswordRequest
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
