using Attendance.Common.Constants;

namespace Attendance.Data.Models.UserModels
{
    public class QualityAssuranceExtension : EmployeeExtension
    {
        public StringEnum.Bands Band { get; set; }
        public bool CanWriteCode { get; set; }
    }
}
