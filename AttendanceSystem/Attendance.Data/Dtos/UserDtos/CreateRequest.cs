using Attendance.Common.Constants;
using System.ComponentModel.DataAnnotations;

namespace Attendance.Data.Dtos.UserDtos
{
    public class CreateRequest
    {
        [Required]
        public string Title { get; set; }

        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required]
        [EnumDataType(typeof(StringEnum.Roles))]
        public StringEnum.Roles Role { get; set; }

        [Required]
        [EnumDataType(typeof(StringEnum.EmployeeTypes))]
        public StringEnum.EmployeeTypes Type { get; set; }

        [Required]
        [EnumDataType(typeof(StringEnum.Sex))]
        public StringEnum.Sex Sex { get; set; }

        [Required]
        [EnumDataType(typeof(StringEnum.Departments))]
        public StringEnum.Departments Department { get; set; }

        [Required]
        public string PhoneNumber { get; set; }

        [Required]
        public bool IsIntern { get; set; }

        [Required]
        public object? Extension { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [MinLength(6)]
        public string Password { get; set; }

        [Required]
        [Compare("Password")]
        public string ConfirmPassword { get; set; }
    }
}
