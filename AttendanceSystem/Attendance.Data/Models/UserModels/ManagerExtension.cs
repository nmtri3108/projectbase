using Attendance.Common.Constants;

namespace Attendance.Data.Models.UserModels
{
    public class ManagerExtension : EmployeeExtension
    {
        public StringEnum.ManagerTypes ManagerType { get; set; }
    }
}
