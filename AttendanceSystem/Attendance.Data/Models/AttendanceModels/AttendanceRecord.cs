using Attendance.Data.Models.UserModels;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Attendance.Data.Models.AttendanceModels
{
    public class AttendanceRecord : BaseEntity
    {
        [Required]
        public DateTime ArrivalTime { get; set; }
        public DateTime? LeaveTime { get; set; }
        [Required]
        public string UserId { get; set; }
        [ForeignKey("UserId")]
        public Account Account { get; set; }
    }
}
