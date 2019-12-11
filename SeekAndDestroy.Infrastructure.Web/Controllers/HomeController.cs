using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SeekAndDestroy.Infrastructure.Web.Models;
using Microsoft.AspNetCore.Authorization;
using SeekAndDestroy.Infrastructure.Web.Extensions;
using SeekAndDestroy.Infrastructure.Web.Api.Controllers;
using SeekAndDestroy.Core.DataAccess;

namespace SeekAndDestroy.Infrastructure.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUserRepository _userRepository;
        private readonly IBuildingsRepository _buildingsRepository;
        private readonly IResourcesRepository _resourcesRepository;

        public HomeController(ILogger<HomeController> logger, IUserRepository userRepository, IBuildingsRepository buildingsRepository, IResourcesRepository resourcesRepository)
        {
            _logger = logger;
            _userRepository = userRepository;
            _buildingsRepository = buildingsRepository;
            _resourcesRepository = resourcesRepository;
        }

        public IActionResult Index()
        {
            return View();
        }

        [Authorize]
        public IActionResult SignIn()
        {
            var userController = new UserController(this.GetCurrentUserIdentity(), _userRepository, _buildingsRepository, _resourcesRepository);

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

        [Authorize]
        public IActionResult Dashboard()
        {
            var userController = new UserController(this.GetCurrentUserIdentity(), _userRepository, _buildingsRepository, _resourcesRepository);
            ViewBag.Resources = userController.GetResources();
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
