using Attendance.Common.Constants;
using Attendance.Data.Context;
using Attendance.Data.Dtos.UserDtos;
using Attendance.Service.IServices;
using Microsoft.EntityFrameworkCore;

namespace Attendance.Api.Initializer
{
    public static class DbInitializer
    {
        public static void Initialize(IApplicationBuilder app)
        {
            using (var serviceScope = app.ApplicationServices.CreateScope())
            {
                var context = serviceScope.ServiceProvider.GetService<ApplicationDbContext>();
                context.Database.EnsureCreated();

                var userService =
                    serviceScope.ServiceProvider.GetService<IUserService>();
                try
                {
                    if (context.Database.GetPendingMigrations().Count() > 0)
                    {
                        context.Database.Migrate();
                    }
                }
                catch (Exception ex)
                {

                }

                if (userService.AdminCheck().GetAwaiter().GetResult())
                    return;

                // user admin
                var user = new CreateRequest()
                {
                    Title = "Mr",
                    FirstName = "Administrator",
                    LastName = "Nguyen",
                    Role = StringEnum.Roles.Administrator,
                    Type = StringEnum.EmployeeTypes.Manager,
                    Email = "Administrator@gmail.com",
                    Password = "Administrator123@",
                    ConfirmPassword = "Administrator123@",
                    Sex = StringEnum.Sex.Male,
                    PhoneNumber = "0342288600",
                    IsIntern = false,
                    Extension = { }
                };

                userService.Create(user).GetAwaiter().GetResult();
            }
        }
    }

}
