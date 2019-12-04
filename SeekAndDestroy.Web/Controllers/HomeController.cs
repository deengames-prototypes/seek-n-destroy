using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SeekAndDestroy.Web.Models;
using Microsoft.AspNetCore.Authorization;
using SeekAndDestroy.Web.Extensions;
using SeekAndDestroy.Web.Api.Controllers;

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
            var userController = new UserController(this.GetCurrentUserIdentity());
            var userId = userController.GetUserId();
            if (userId == 0)
            {
                userController.CreateNewUser();
                ViewBag.Welcome = "noob";
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
