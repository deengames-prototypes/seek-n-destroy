using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Authorization;
using SeekAndDestroy.Web.Extensions;
using Npgsql;
using Dapper;
using System.Security.Claims;
using Microsoft.Extensions.Configuration;

namespace SeekAndDestroy.Web.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private ClaimsIdentity identity;
        private const string EMAIL_ADDRESS_CLAIM = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress";
        public UserController(ClaimsIdentity userIdentity)
        {
            identity = userIdentity;
        }

        [Authorize]
        [HttpPost]
        public void CreateNewUser()
        {
            using (var connection = new NpgsqlConnection(this.GetAppConfig()["ConnectionString"]))
                {
                    var emailAddress = identity.Claims.Single(c => c.Type == EMAIL_ADDRESS_CLAIM).Value;
                    connection.Execute("INSERT INTO users (oauth_id, email_address) VALUES (@oauth2id, @emailAddress);", new { oauth2id = this.GetOAuth2Id(), emailAddress });
                    var userId = this.GetUserId();

                    connection.Execute("INSERT INTO buildings VALUES (@user_id, @starting_crystal_factories);", new { user_id = userId, starting_crystal_factories = 1});
                    connection.Execute("INSERT INTO resources VALUES (@user_id, @starting_crystals);", new { user_id = userId, starting_crystals = 0});
                }
        }

        // TODO: move these methods somewhere more appropriate
        public string GetOAuth2Id()
        {
            // Claims include: a unique identifier; first name, last name, picture, locale, and more.
            // nameidentifier is the definitive ID; see: https://stackoverflow.com/a/43064583
            return identity.Claims.Single(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier").Value;
        }

        /// <summary>
        /// Gets the current user ID; returns 0 if we don't have a record for them in the users table.
        /// Assumes they're already authenticated.
        /// </summary>

        public int GetUserId()
        {
            // Assumes the user is authenticated. Returns the user's ID (our ID).
            // Step 1: look up the nameidentifier value (OAuth ID)
            var oauthId = this.GetOAuth2Id();
            
            // Step 2: cross-reference the Users table to get our ID
            // TODO: proper data-access layers (repository pattern?)
            using (var connection = new NpgsqlConnection(this.GetAppConfig()["ConnectionString"]))
            {
                var userId = connection.ExecuteScalar<int>("SELECT user_id FROM users WHERE oauth_id = @oauthId", new { oauthId });
                return userId;
            }
        }

        public IConfigurationRoot GetAppConfig()
        {
            return new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
        }
    }
}
