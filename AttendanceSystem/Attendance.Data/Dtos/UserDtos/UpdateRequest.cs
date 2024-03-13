using Attendance.Common.Constants;
using Attendance.Data.Models.UserModels;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace Attendance.Data.Dtos.UserDtos
{
    public class UpdateRequest
    {
        private string? _password;

        private string? _confirmPassword;

        [EnumDataType(typeof(StringEnum.Roles))]
        public StringEnum.Roles Role { get; set; }

        public StringEnum.EmployeeTypes Type { get; set; }

        private string? _email;

        public string Title { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public StringEnum.Sex Sex { get; set; }

        public StringEnum.Departments Department { get; set; }

        public string PhoneNumber { get; set; }

        public bool IsIntern { get; set; }

        public object? Extension { get; set; }

        [EmailAddress]
        public string? Email
        {
            get => _email;
            set => _email = ReplaceEmptyWithNull(value);
        }

        [MinLength(6)]
        public string? Password
        {
            get => _password;
            set => _password = ReplaceEmptyWithNull(value);
        }

        [Compare("Password")]
        public string? ConfirmPassword
        {
            get => _confirmPassword;
            set => _confirmPassword = ReplaceEmptyWithNull(value);
        }

        // helpers

        private string? ReplaceEmptyWithNull(string? value)
        {
            // replace empty string with null to make field optional
            return String.IsNullOrEmpty(value) ? null : value;
        }
    }
}
