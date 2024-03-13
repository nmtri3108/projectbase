using Attendance.Api.CustomAttributes;
using Attendance.Common.Constants;
using Attendance.Data.Dtos.UserDtos;
using Attendance.Data.Models.AttendanceModels;
using Attendance.Service.IServices;
using Attendance.Service.Services;
using Microsoft.AspNetCore.Mvc;

namespace Attendance.Api.Controllers
{
    [Authorize]
    public class AttendanceController : BaseController
    {
        private readonly IAttendanceService attendanceService;
        public AttendanceController(IAttendanceService attendanceService)
        {
            this.attendanceService = attendanceService;
        }

        [HttpPost("{type}")]
        public async Task<IActionResult> AddAttendanceRecord([FromRoute] StringEnum.AttendanceRecordType type)
        {
            await attendanceService.AddAttendanceAsync(Account.Id, type);
            return Ok();
        }

        [HttpGet]
        public async Task<IActionResult> GetEmployeeAttendances()
        {
            var attendanceRecords = await attendanceService.GetAttendanceRecordsByEmployeeAsync(Account.Id);
            if (attendanceRecords.Any())
            {
                return Ok(attendanceRecords);
            }
            return NotFound();
        }

        [Authorize(StringEnum.Roles.Manager)]
        [HttpGet("managed")]
        public async Task<IActionResult> GetManagerAttendanceRecords()
        {
            var result = await attendanceService.GetManagedAttendanceRecords(Account.Id);
            if (result.Any())
            {
                return Ok(result);
            }
            return NotFound();
        }

        [Authorize(StringEnum.Roles.Manager)]
        [HttpGet("object-fields")]
        public IActionResult GetObjectFields()
        {
            List<string> availableFields = attendanceService.GetFields();
            return Ok(availableFields);
        }

        [Authorize(StringEnum.Roles.Manager)]
        [HttpPost("read-excel")]
        public async Task<IActionResult> ReadExcelFile([FromForm] UploadExcelModel model)
        {
            try
            {
                if (model.file == null || model.file.Length == 0)
                {
                    return BadRequest("No file uploaded.");
                }

                var result = await attendanceService.ReadExcel(model.file);
                return Ok(result);
            }
            catch (Exception ex)
            {
                // Handle exceptions and return an error response
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [Authorize(StringEnum.Roles.Manager)]
        [HttpPost("import")]
        public async Task<ActionResult> Import([FromForm] ImportExcelModel model)
        {
            try
            {
                if (model.file == null || model.file.Length == 0)
                {
                    return BadRequest("No file uploaded.");
                }

               await attendanceService.ImportExcel(model.file, model.mapping);
                return Ok();
            }
            catch (Exception ex)
            {
                // Handle exceptions and return an error response
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
