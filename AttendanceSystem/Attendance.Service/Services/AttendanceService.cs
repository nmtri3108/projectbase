using Attendance.Common.Constants;
using Attendance.Data.Context;
using Attendance.Data.Dtos.AttendanceDtos;
using Attendance.Data.Dtos.UserDtos;
using Attendance.Data.Models.AttendanceModels;
using Attendance.Data.Models.UserModels;
using Attendance.Service.IServices;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OfficeOpenXml;
using System.Data;

namespace Attendance.Service.Services
{
    public class AttendanceService : IAttendanceService
    {
        private readonly ILogger<AttendanceService> logger;
        private readonly IUserService userService;
        private readonly ApplicationDbContext db;
        private readonly IMapper mapper;

        public AttendanceService(
            ILogger<AttendanceService> logger,
            IUserService userService,
            ApplicationDbContext db,
            IMapper mapper
            )
        {
            this.logger = logger;
            this.userService = userService;
            this.db = db;
            this.mapper = mapper;
        }

        public async Task AddAttendanceAsync(string employeeId, StringEnum.AttendanceRecordType type)
        {
            var time = DateTime.UtcNow;
            var date = time.Date;

            var currentRecords = await db.Attendances.FirstOrDefaultAsync(_ => _.UserId == employeeId && _.Created.Date == date);
            if (currentRecords == null)
            {
                currentRecords = new AttendanceRecord
                {
                    UserId = employeeId,
                    Created = time
                };

                SetRecordTime(currentRecords, type, time);

                await db.Attendances.AddAsync(currentRecords);
                await db.SaveChangesAsync();
            }
            else
            {
                SetRecordTime(currentRecords, type, time);
                await db.SaveChangesAsync();
            }
        }

        private void SetRecordTime(AttendanceRecord record, StringEnum.AttendanceRecordType type, DateTime time)
        {
            if (type == StringEnum.AttendanceRecordType.Arrival)
            {
                record.ArrivalTime = time;
            }
            else
            {
                record.LeaveTime = time;
            }
        }

        public async Task<List<AttendanceReponse>> GetAttendanceRecordsByEmployeeAsync(string employeeId)
        {
            var result = await db.Attendances.Where(_ => _.UserId == employeeId).OrderByDescending(x => x.Created).ToListAsync();

            return mapper.Map<List<AttendanceReponse>>(result);
        }

        public async Task<List<ManagedAttendanceRecordResponse>> GetManagedAttendanceRecords(string managerId)
        {
            var result = new List<ManagedAttendanceRecordResponse>();
            var manager = await userService.GetById(managerId);

            if (manager.Extension is ManagerExtension managerExtension)
            {
                var employees = new List<AccountResponse>();
                var records = new List<AttendanceRecord>();

                if (managerExtension.ManagerType == StringEnum.ManagerTypes.DepartmentManager)
                {
                    employees = (await userService.GetByDempartment(manager.Department)).ToList();
                    records = await db.Attendances.Include(_ => _.Account).Where(_ => _.Account.Department == manager.Department).ToListAsync();
                }
                else
                {
                    employees = (await userService.GetAll()).ToList();
                    records = await db.Attendances.ToListAsync();
                }



                if (records.Any())
                {
                    var recordGroups = records.GroupBy(i => i.UserId);
                    foreach (var group in recordGroups)
                    {
                        var employee = employees.FirstOrDefault(i => String.Equals(i.Id, group.Key));
                        if (employee != null)
                        {
                            var item = new ManagedAttendanceRecordResponse();
                            item.User = employee;
                            item.AttendanceRecords = mapper.Map<List<AttendanceReponse>>(group.OrderByDescending(i => i.Created).ToList());
                            result.Add(item);
                        }
                        else
                        {
                            this.logger.LogWarning($"Cannot find employee with id: {group.Key}");
                        }
                    }
                }
                return result;
            }

            return result;
        }

        public List<string> GetFields()
        {
            return new List<string>()
            {
               "Created","ArrivalTime", "LeaveTime", "Email"
            };
        }

        public async Task<List<Dictionary<string, string>>> ReadExcel(IFormFile file)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using (var package = new ExcelPackage(file.OpenReadStream()))
            {
                var worksheet = package.Workbook.Worksheets[0];

                // Extract data into a list of dictionaries
                var data = new List<Dictionary<string, string>>();
                for (var rowNumber = 1; rowNumber <= worksheet.Dimension.End.Row; rowNumber++)
                {
                    var row = worksheet.Cells[rowNumber, 1, rowNumber, worksheet.Dimension.End.Column];
                    var rowData = new Dictionary<string, string>();
                    foreach (var cell in row)
                    {
                        rowData[cell.Start.Column.ToString()] = cell.Text;
                    }

                    data.Add(rowData);
                }

                return data;
            }
        }

        public async Task ImportExcel(IFormFile file, string mapping)
        {
            // Load the Excel file using EPPlus
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using (var package = new ExcelPackage(file.OpenReadStream()))
            {
                var worksheet = package.Workbook.Worksheets[0];

                // Extract data into a DataTable
                DataTable dt = new DataTable();
                foreach (var firstRowCell in worksheet.Cells[1, 1, 1, worksheet.Dimension.End.Column])
                {
                    dt.Columns.Add(firstRowCell.Text);
                }

                for (var rowNumber = 2; rowNumber <= worksheet.Dimension.End.Row; rowNumber++)
                {
                    var row = worksheet.Cells[rowNumber, 1, rowNumber, worksheet.Dimension.End.Column];
                    var newRow = dt.NewRow();
                    var count = 0;
                    foreach (var cell in row)
                    {
                        newRow[count] = cell.Text;
                        count++;
                    }

                    dt.Rows.Add(newRow);
                }

                // Deserialize the mapping string into a C# object
                var userMapping =
                    Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, string>>(mapping);

                // Map Excel columns to appropriate fields
                var mappedData = new List<ExcelUploadAttendance>();
                foreach (DataRow row in dt.Rows)
                {
                    var item = new ExcelUploadAttendance();

                    foreach (var mappingEntry in userMapping)
                    {
                        var excelColumn = mappingEntry.Value;
                        var dataField = mappingEntry.Key;

                        // Map Excel data to the corresponding field
                        if (dt.Columns.Contains(excelColumn))
                        {
                            var value = row[excelColumn].ToString();

                            var propertyInfo = typeof(ExcelUploadAttendance).GetProperty(dataField);

                            if (propertyInfo?.PropertyType == typeof(DateTime) || propertyInfo?.PropertyType == typeof(DateTime?))
                            {
                                DateTime dateTime;
                                if (DateTime.TryParseExact(value, "M/d/yy H:mm",
                                    System.Globalization.CultureInfo.InvariantCulture,
                                    System.Globalization.DateTimeStyles.None, out DateTime localDateTime))
                                {
                                    // Convert local date-time to UTC
                                    dateTime = localDateTime.ToUniversalTime();

                                    propertyInfo.SetValue(item, dateTime);
                                }
                                else
                                {
                                    propertyInfo.SetValue(item, true);
                                }
                            }
                            else
                            {
                                propertyInfo?.SetValue(item, value);
                            }
                        }
                    }

                    mappedData.Add(item);
                }

                await ImportAttendances(mappedData);

            }
        }

        #region Helpers
        private async Task ImportAttendances(List<ExcelUploadAttendance> input)
        {
            var accounts = await db.Accounts.ToListAsync();
            var validData = new List<AttendanceRecord>();

            // Process and validate the data as needed and save
            foreach (var model in input)
            {
                if (!CheckValidData(model))
                    continue;

                var account = accounts.FirstOrDefault(_ => _.Email.ToLower().Trim() == model.Email.ToLower().Trim());
                if (account == null)
                    continue;

                var attendance = new AttendanceRecord();
                attendance.ArrivalTime = model.ArrivalTime;
                attendance.LeaveTime = model.LeaveTime;
                attendance.Created = model.Created;
                attendance.UserId = account.Id;

                validData.Add(attendance);
            }

            await db.Attendances.AddRangeAsync(validData);
            await db.SaveChangesAsync();
        }

        private bool CheckValidData(ExcelUploadAttendance attendance)
        {
            var basicValidation = !string.IsNullOrEmpty(attendance.Email)
                                  && attendance.LeaveTime != null;

            return basicValidation;
        }
        #endregion
    }
}
