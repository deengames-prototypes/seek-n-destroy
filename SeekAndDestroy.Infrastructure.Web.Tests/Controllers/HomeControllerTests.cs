using System.Collections.Generic;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SeekAndDestroy.Infrastructure.Web.Controllers;

namespace SeekAndDestroy.Infrastructure.Web.Tests.Controllers
{
    [TestFixture]
    public class HomeControllerTests
    {
        private const string OAUTH_ID_CLAIM = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier";
        private const string EMAIL_ADDRESS_CLAIM = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress";

        [Test]
        public void HelloWorld()
        {
            var controller = new HomeController(new Mock<ILogger<HomeController>>().Object);

            // Spoof ID
            var identities = new List<ClaimsIdentity>()
            {
                new ClaimsIdentity(new List<Claim>() {
                    new Claim(OAUTH_ID_CLAIM, "1234"),
                    new Claim(EMAIL_ADDRESS_CLAIM, "test@test.com"),

                    
                }, "AuthenticationTypes.Federation"),
            };

            var httpContext = new DefaultHttpContext()
            {
                User = new System.Security.Claims.ClaimsPrincipal(identities)
            };

            controller.ControllerContext = new ControllerContext();
            controller.ControllerContext.HttpContext = httpContext;
            //controller.ControllerContext.HttpContext.Request.Headers["device-id"] = "20317";

            controller.SignIn();
        }
    }
}