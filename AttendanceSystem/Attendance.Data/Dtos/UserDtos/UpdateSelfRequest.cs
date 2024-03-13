using Attendance.Common.Constants;
using System.ComponentModel.DataAnnotations;

namespace Attendance.Data.Dtos.UserDtos
{
    public class UpdateSelfRequest
    {
        private string? _password;

        private string? _confirmPassword;

        public string Title { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public StringEnum.Sex Sex { get; set; }
        public string PhoneNumber { get; set; }

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
