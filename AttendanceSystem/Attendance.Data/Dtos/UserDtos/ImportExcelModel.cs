using Microsoft.AspNetCore.Http;

namespace Attendance.Data.Dtos.UserDtos
{
    public class ImportExcelModel
    {
        public IFormFile file { get; set; }
        public string mapping { get; set; }
    }
}
