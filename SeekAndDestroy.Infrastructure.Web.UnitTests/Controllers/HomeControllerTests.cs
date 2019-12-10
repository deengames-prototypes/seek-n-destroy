using System.Collections.Generic;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SeekAndDestroy.Core.DataAccess;
using SeekAndDestroy.Infrastructure.Web.Controllers;

namespace SeekAndDestroy.Infrastructure.Web.UnitTests.Controllers
{
    [TestFixture]
    public class HomeControllerTests
    {
        private const string OAUTH_ID_CLAIM = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier";
        private const string EMAIL_ADDRESS_CLAIM = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress";

        [Test]
        public void SignInCallsCreateNewUserOnApiControllerIfUserIdIsZero()
        {
            // Arrange
            const string OauthId = "1234";
            const string EmailAddress = "test@test.com";

            var userRepository = new Mock<IUserRepository>();
            var buildingsRepository = new Mock<IBuildingsRepository>();
            var resourcesRepository = new Mock<IResourcesRepository>();

            var controller = new HomeController(new Mock<ILogger<HomeController>>().Object, userRepository.Object, buildingsRepository.Object, resourcesRepository.Object);

            // Spoof ID
            var identities = new List<ClaimsIdentity>()
            {
                new ClaimsIdentity(new List<Claim>() {
                    new Claim(OAUTH_ID_CLAIM, OauthId),
                    new Claim(EMAIL_ADDRESS_CLAIM, EmailAddress),
                }, "AuthenticationTypes.Federation"),
            };

            var httpContext = new DefaultHttpContext()
            {
                User = new System.Security.Claims.ClaimsPrincipal(identities)
            };

            // Modified from https://stackoverflow.com/a/41400246/8641842
            controller.ControllerContext = new ControllerContext();
            controller.ControllerContext.HttpContext = httpContext;

            // Act
            controller.SignIn();

            // Assert
            userRepository.Verify(u => u.CreateUser(OauthId, EmailAddress));
        }

        [Test]
        public void SignInDoesNotCallCreateNewUserOnApiControllerIfUserIdIsZero()
        {
            
        }
    }
}