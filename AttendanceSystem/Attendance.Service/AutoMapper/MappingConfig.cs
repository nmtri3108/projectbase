using Attendance.Data.Dtos.UserDtos;
using AutoMapper;
using Attendance.Data.Models.UserModels;
using Attendance.Data.Models.AttendanceModels;
using Attendance.Data.Dtos.AttendanceDtos;

namespace Attendance.Service.AutoMapper
{
    public class MappingConfig
    {
        public static MapperConfiguration RegisterMaps()
        {
            var mappingConfig = new MapperConfiguration(config =>
            {
                // mapping attendance
                config.CreateMap<AttendanceRecord, AttendanceReponse>();
                // mapping user

                config.CreateMap<Account, AccountResponse>();

                config.CreateMap<Account, AuthenticateResponse>();

                config.CreateMap<CreateRequest, Account>()
                .ForAllMembers(x => x.Condition(
                        (src, dest, prop) =>
                        {
                            // ignore null & empty string properties
                            if (prop == null) return false;
                            if (prop.GetType() == typeof(string) && string.IsNullOrEmpty((string)prop)) return false;

                            // ignore null role
                            if (x.DestinationMember.Name == "Role" && src.Role == null) return false;

                            return true;
                        }));

                config.CreateMap<RegisterRequest, Account>();

                config.CreateMap<UpdateRequest, Account>()
                    .ForAllMembers(x => x.Condition(
                        (src, dest, prop) =>
                        {
                            // ignore null & empty string properties
                            if (prop == null) return false;
                            if (prop.GetType() == typeof(string) && string.IsNullOrEmpty((string)prop)) return false;

                            // ignore null role
                            if (x.DestinationMember.Name == "Role" && src.Role == null) return false;

                            // ignore null role
                            if (x.DestinationMember.Name == "Extension") return false;

                            return true;
                        }
                    ));
                config.CreateMap<UpdateSelfRequest, Account>()
                    .ForAllMembers(x => x.Condition(
                        (src, dest, prop) =>
                        {
                            // ignore null & empty string properties
                            if (prop == null) return false;
                            if (prop.GetType() == typeof(string) && string.IsNullOrEmpty((string)prop)) return false;

                            return true;
                        }
                    ));
            });
            return mappingConfig;
        }

    }
}
