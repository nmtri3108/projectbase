using Attendance.Common.Constants;
using Attendance.Data.Dtos.AttendanceDtos;
using Microsoft.AspNetCore.Http;

namespace Attendance.Service.IServices
{
    public interface IAttendanceService
    {
        Task AddAttendanceAsync(string employeeId, StringEnum.AttendanceRecordType type);
        Task<List<AttendanceReponse>> GetAttendanceRecordsByEmployeeAsync(string employeeId);
        Task<List<ManagedAttendanceRecordResponse>> GetManagedAttendanceRecords(string managerId);
        List<string> GetFields();
        Task<List<Dictionary<string, string>>> ReadExcel(IFormFile file);
        Task ImportExcel(IFormFile file, string mapping);
    }
}
