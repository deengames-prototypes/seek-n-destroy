using System.Linq;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace SeekAndDestroy.Infrastructure.Web.Extensions
{
    public static class ControllerExtensions
    {
        public static ClaimsIdentity GetCurrentUserIdentity(this ControllerBase controller)
        {
            return controller.HttpContext.User.Identities
                .Single(i => i.AuthenticationType == "AuthenticationTypes.Federation");
        }
    }
}