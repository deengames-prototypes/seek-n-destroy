using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Dapper;
using Microsoft.Extensions.Configuration;
using Npgsql;
using System.Security.Claims;

namespace SeekAndDestroy.Web.Extensions
{
    public static class ControllerExtensions
    {
        public static ClaimsIdentity GetCurrentUserIdentity(this Controller controller)
        {
            return controller.HttpContext.User.Identities
                .Single(i => i.AuthenticationType == "AuthenticationTypes.Federation");
        }
    }
}