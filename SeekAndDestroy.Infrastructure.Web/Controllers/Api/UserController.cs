using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Npgsql;
using Dapper;
using System.Security.Claims;
using Microsoft.Extensions.Configuration;
using SeekAndDestroy.Core.DataAccess;

namespace SeekAndDestroy.Infrastructure.Web.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private const string OAUTH_ID_CLAIM = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier";
        private const string EMAIL_ADDRESS_CLAIM = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress";

        private ClaimsIdentity _identity;
        private IUserRepository _userRepository;
        private IBuildingsRepository _buildingsRepository;
        private IResourcesRepository _resourcesRepository;
        private readonly IConfiguration _configuration;

        public UserController(ClaimsIdentity identity, IUserRepository userRepository, IBuildingsRepository buildingsRepository, IResourcesRepository resourcesRepository)
        {
            _identity = identity;
            _userRepository = userRepository;
            _buildingsRepository = buildingsRepository;
            _resourcesRepository = resourcesRepository;
            _configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
        }

        [Authorize]
        [HttpPost]
        public void CreateNewUser()
        {
            var oauthId = _identity.Claims.Single(c => c.Type == OAUTH_ID_CLAIM).Value;
            var emailAddress = _identity.Claims.Single(c => c.Type == EMAIL_ADDRESS_CLAIM).Value;

            var userId = _userRepository.CreateUser(oauthId, emailAddress);
            _buildingsRepository.InitializeForUser(userId);
            _resourcesRepository.InitializeForUser(userId);
        }

        // TODO: move these methods somewhere more appropriate
        public string GetOAuth2Id()
        {
            // Claims include: a unique identifier; first name, last name, picture, locale, and more.
            // nameidentifier is the definitive ID; see: https://stackoverflow.com/a/43064583
            return _identity.Claims.Single(c => c.Type == OAUTH_ID_CLAIM).Value;
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
            using (var connection = new NpgsqlConnection(_configuration["ConnectionString"]))
            {
                // TODO: user repository pl0x
                var userId = connection.ExecuteScalar<int>("SELECT user_id FROM users WHERE oauth_id = @oauthId", new { oauthId });
                return userId;
            }
        }
    }
}
