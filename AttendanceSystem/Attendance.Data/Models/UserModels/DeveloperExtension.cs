using Attendance.Common.Constants;

namespace Attendance.Data.Models.UserModels
{
    public class DeveloperExtension : EmployeeExtension
    {
        public StringEnum.Bands Band { get; set; }
        public string TechDirection { get; set; }
    }
}
