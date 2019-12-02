using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Dapper;
using Microsoft.Extensions.Configuration;
using Npgsql;

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
            // nameidentifier is the definitive ID; see: https://stackoverflow.com/a/43064583
            var oauthId = controller.HttpContext.User.Identities
                .Single(i => i.AuthenticationType == "AuthenticationTypes.Federation")
                // Claims include: a unique identifier; first name, last name, picture, locale, and more.
                .Claims.Single(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier").Value;
            
            // Step 2: cross-reference the Users table to get our ID
            // TODO: proper data-access layers (repository pattern?)
            var configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
            var connectionString = configuration["ConnectionString"];
            using (var connection = new NpgsqlConnection(connectionString))
            {
                var userId = connection.ExecuteScalar<int>("SELECT user_id FROM users WHERE oauth_id = @oauthId", new { oauthId });
                return userId;
            }
        }
    }
}