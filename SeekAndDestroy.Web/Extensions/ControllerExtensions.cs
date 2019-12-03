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
        /// <summary>
        /// Gets the current user ID; returns 0 if we don't have a record for them in the users table.
        /// Assumes they're already authenticated.
        /// </summary>

        public static int GetCurrentUserId(this Controller controller)
        {
            // Assumes the user is authenticated. Returns the user's ID (our ID).
            // Step 1: look up the nameidentifier value (OAuth ID)
            var oauthId = controller.GetCurrentUserOAuth2Id();
            
            // Step 2: cross-reference the Users table to get our ID
            // TODO: proper data-access layers (repository pattern?)
            using (var connection = new NpgsqlConnection(controller.GetAppConfig()["ConnectionString"]))
            {
                var userId = connection.ExecuteScalar<int>("SELECT user_id FROM users WHERE oauth_id = @oauthId", new { oauthId });
                return userId;
            }
        }

        public static ClaimsIdentity GetCurrentUserIdentity(this Controller controller)
        {
            return controller.HttpContext.User.Identities
                .Single(i => i.AuthenticationType == "AuthenticationTypes.Federation");
        }

        public static string GetCurrentUserOAuth2Id(this Controller controller)
        {
            // Claims include: a unique identifier; first name, last name, picture, locale, and more.
            // nameidentifier is the definitive ID; see: https://stackoverflow.com/a/43064583
            return controller.GetCurrentUserIdentity()
            .Claims.Single(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier").Value;
        }

        public static IConfigurationRoot GetAppConfig(this Controller controller)
        {
            return new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
        }
    }
}