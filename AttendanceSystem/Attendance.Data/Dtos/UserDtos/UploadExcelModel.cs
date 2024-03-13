using Microsoft.AspNetCore.Http;

namespace Attendance.Data.Dtos.UserDtos
{
    public class UploadExcelModel
    {
        public IFormFile file { get; set; }
    }
}
