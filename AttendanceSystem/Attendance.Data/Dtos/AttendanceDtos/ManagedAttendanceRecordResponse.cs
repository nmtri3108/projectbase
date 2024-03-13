using Attendance.Data.Dtos.UserDtos;

namespace Attendance.Data.Dtos.AttendanceDtos
{
    public class ManagedAttendanceRecordResponse
    {
        public AccountResponse User { get; set; }
        public List<AttendanceReponse> AttendanceRecords { get; set; }
    }
}
