using Attendance.Common.Constants;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Attendance.Data.Models.UserModels
{
    public class Account : BaseEntity
    {
        public Account()
        {
            RefreshTokens = new List<RefreshToken>();
        }

        public string? Title { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public string PasswordHash { get; set; }
        public bool AcceptTerms { get; set; }
        public StringEnum.Roles Role { get; set; }
        public StringEnum.EmployeeTypes Type { get; set; }
        public string? VerificationToken { get; set; }
        public DateTime? Verified { get; set; }
        public bool IsVerified => Verified.HasValue || PasswordReset.HasValue;
        public string? ResetToken { get; set; }
        public DateTime? ResetTokenExpires { get; set; }
        public DateTime? PasswordReset { get; set; }
        public DateTime? Updated { get; set; }
        public List<RefreshToken> RefreshTokens { get; set; }

        public bool OwnsToken(string token)
        {
            return RefreshTokens?.Find(x => x.Token == token) != null;
        }

        public StringEnum.Sex Sex { get; set; }
        public StringEnum.Departments? Department { get; set; }
        public string PhoneNumber { get; set; }
        public bool IsIntern { get; set; }

        public string? SerializedExtension { get; set; }

        [NotMapped]
        public EmployeeExtension? Extension
        {
            get
            {
                if (string.IsNullOrEmpty(SerializedExtension))
                {
                    return null;
                }

                switch (Type)
                {
                    case StringEnum.EmployeeTypes.Dev:
                        return JsonConvert.DeserializeObject<DeveloperExtension>(SerializedExtension, new JsonSerializerSettings
                        {
                            TypeNameHandling = TypeNameHandling.Auto,
                        });
                    case StringEnum.EmployeeTypes.QA:
                        return JsonConvert.DeserializeObject<QualityAssuranceExtension>(SerializedExtension, new JsonSerializerSettings
                        {
                            TypeNameHandling = TypeNameHandling.Auto,
                        });
                    case StringEnum.EmployeeTypes.Manager:
                        return JsonConvert.DeserializeObject<ManagerExtension>(SerializedExtension, new JsonSerializerSettings 
                        {
                            TypeNameHandling = TypeNameHandling.Auto 
                        });
                    default:
                        throw new ArgumentException("Invalid role type");
                }
            }
            set => SerializedExtension = JsonConvert.SerializeObject(value);
        }
    }
}
