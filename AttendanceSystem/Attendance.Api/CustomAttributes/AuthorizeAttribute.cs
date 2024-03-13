using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using Attendance.Common.Constants;
using Attendance.Data.Models.UserModels;

namespace Attendance.Api.CustomAttributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class AuthorizeAttribute : Attribute, IAuthorizationFilter
    {
        private readonly IList<StringEnum.Roles> roles;
        public AuthorizeAttribute(params StringEnum.Roles[] roles)
        {
            this.roles = roles ?? new StringEnum.Roles[] { };
        }
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            // skip authorization if action is decorated with [AllowAnonymous] attribute
            var allowAnonymous = context.ActionDescriptor.EndpointMetadata.OfType<AllowAnonymousAttribute>().Any();
            if (allowAnonymous)
                return;

            // authorization
            var user = (Account)context.HttpContext.Items["Account"];
            if (user == null || (this.roles.Any() && !this.roles.Contains(user.Role)))
                context.Result = new JsonResult(new { message = "Unauthorized" }) { StatusCode = StatusCodes.Status401Unauthorized };
        }
    }
}
