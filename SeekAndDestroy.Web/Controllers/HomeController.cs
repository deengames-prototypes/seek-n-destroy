using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SeekAndDestroy.Web.Models;
using Microsoft.AspNetCore.Authorization;
using SeekAndDestroy.Web.Extensions;
using Dapper;
using System.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Npgsql;
using System.Linq;

namespace SeekAndDestroy.Web.Controllers
{
    public class HomeController : Controller
    {
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
            var configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
                var identity = this.HttpContext.User.Identities
                    .Single(i => i.AuthenticationType == "AuthenticationTypes.Federation");
                var oauth2Id = identity.Claims
                    .Single(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier").Value;
                ViewBag.Welcome = "noob";

                using (var connection = new NpgsqlConnection(configuration["ConnectionString"]))
                {
                    connection.Execute("INSERT INTO users (oauth_id) VALUES (@oauth2id);", new { oauth2id = oauth2Id });
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
