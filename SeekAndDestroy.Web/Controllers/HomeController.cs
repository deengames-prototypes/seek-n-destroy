using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SeekAndDestroy.Web.Models;
using Microsoft.AspNetCore.Authorization;
using SeekAndDestroy.Web.Api.Controllers;
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
        public IActionResult Secured()
        {
            var apiValues = new ValuesController().Get();
            ViewBag.apiResults = apiValues;
            // nameidentifier is the definitive ID; see: https://stackoverflow.com/a/43064583 and https://docs.microsoft.com/en-us/azure/active-directory/develop/active-directory-token-and-claims
            var oauthId = this.HttpContext.User.Identities
                .Single(i => i.AuthenticationType == "AuthenticationTypes.Federation")
                // Claims include: a unique identifier; first name, last name, picture, locale, and more.
                .Claims.Single(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier").Value;
            
            ViewBag.OAuthId = oauthId;
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
