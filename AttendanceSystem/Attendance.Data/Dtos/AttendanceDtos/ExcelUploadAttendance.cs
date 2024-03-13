namespace Attendance.Data.Dtos.AttendanceDtos
{
    public class ExcelUploadAttendance
    {
        public DateTime Created { get; set; }
        public DateTime ArrivalTime { get; set; }
        public DateTime? LeaveTime { get; set; }
        public string Email { get; set; }
    }
}
