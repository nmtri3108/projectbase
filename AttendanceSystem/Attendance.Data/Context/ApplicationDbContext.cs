using Attendance.Data.Models.AttendanceModels;
using Attendance.Data.Models.UserModels;
using Microsoft.EntityFrameworkCore;

namespace Attendance.Data.Context
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        { }

        public DbSet<Account> Accounts { get; set; }
        public DbSet<AttendanceRecord> Attendances { get; set; }
    }
}
