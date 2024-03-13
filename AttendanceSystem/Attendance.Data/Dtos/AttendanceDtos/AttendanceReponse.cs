namespace Attendance.Data.Dtos.AttendanceDtos
{
    public class AttendanceReponse
    {
        public string Id { get; set; }
        public DateTime Created { get; set; }
        public DateTime ArrivalTime { get; set; }
        public DateTime? LeaveTime { get; set; }
        public string UserId { get; set; }
    }
}
