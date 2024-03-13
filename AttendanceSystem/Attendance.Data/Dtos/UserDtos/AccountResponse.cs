using Attendance.Common.Constants;
using Attendance.Data.Models.UserModels;

namespace Attendance.Data.Dtos.UserDtos
{
    public class AccountResponse
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public StringEnum.Roles Role { get; set; }
        public StringEnum.EmployeeTypes Type { get; set; }
        public DateTime Created { get; set; }
        public DateTime? Updated { get; set; }
        public bool IsVerified { get; set; }
        public StringEnum.Sex Sex { get; set; }
        public StringEnum.Departments Department { get; set; }
        public string PhoneNumber { get; set; }
        public bool IsIntern { get; set; }
        public EmployeeExtension? Extension { get; set; }
    }
}
