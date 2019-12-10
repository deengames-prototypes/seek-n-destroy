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
            controller.MockUserClaims(OauthId, EmailAddress);            

            // Act
            controller.SignIn();

            // Assert
            userRepository.Verify(u => u.CreateUser(OauthId, EmailAddress));
        }

        [Test]
        public void SignInDoesNotCallCreateNewUserOnApiControllerIfUserIdIsNonZero()
        {
            // Arrange
            const string OauthId = "1234";
            const string EmailAddress = "test@test.com";

            var userRepository = new Mock<IUserRepository>();
            var buildingsRepository = new Mock<IBuildingsRepository>();
            var resourcesRepository = new Mock<IResourcesRepository>();

            userRepository.Setup(u => u.GetUserId(OauthId)).Returns(13);

            var controller = new HomeController(new Mock<ILogger<HomeController>>().Object, userRepository.Object, buildingsRepository.Object, resourcesRepository.Object);
            controller.MockUserClaims(OauthId, EmailAddress);            

            // Act
            controller.SignIn();

            // Assert
            userRepository.Verify(u => u.CreateUser(OauthId, EmailAddress), Times.Never());
        }
    }
}