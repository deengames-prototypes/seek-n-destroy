using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SeekAndDestroy.Web.Models;
using Microsoft.AspNetCore.Authorization;
using SeekAndDestroy.Web.Extensions;
using Dapper;
using Npgsql;
using System.Linq;

namespace SeekAndDestroy.Web.Controllers
{
    public class HomeController : Controller
    {
        private const string EMAIL_ADDRESS_CLAIM = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress";

        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        [Authorize]
        public IActionResult SignIn()
        {
            var userId = this.GetCurrentUserId();
            if (userId == 0)
            {
                
                ViewBag.Welcome = "noob";
                var emailAddress = this.GetCurrentUserIdentity().Claims.Single(c => c.Type == EMAIL_ADDRESS_CLAIM).Value;

                using (var connection = new NpgsqlConnection(this.GetAppConfig()["ConnectionString"]))
                {
                    connection.Execute("INSERT INTO users (oauth_id, email_address) VALUES (@oauth2id, @emailAddress);", new { oauth2id = this.GetCurrentUserOAuth2Id(), emailAddress });
                    userId = this.GetCurrentUserId();

                    connection.Execute("INSERT INTO buildings VALUES (@user_id, @starting_crystal_factories);", new { user_id = userId, starting_crystal_factories = 1});
                    connection.Execute("INSERT INTO resources VALUES (@user_id, @starting_crystals);", new { user_id = userId, starting_crystals = 0});
                }
            }
            else
            {
                ViewBag.Welcome = $"User {userId}";
            }
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
