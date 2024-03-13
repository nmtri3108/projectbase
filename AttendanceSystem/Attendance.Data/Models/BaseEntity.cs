using System.ComponentModel.DataAnnotations;

namespace Attendance.Data.Models
{
    public abstract class BaseEntity
    {
        protected BaseEntity()
        {
            Id = Guid.NewGuid().ToString();
            Created = DateTime.UtcNow;
        }

        [Key]
        public string Id { get; set; }
        public DateTime Created { get; set; }
    }
}
