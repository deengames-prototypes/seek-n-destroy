using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SeekAndDestroy.Core.DataAccess;
using SeekAndDestroy.Core.Enums;
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

        [Test]
        public void DashboardSetsViewBagResourcesFromUserController()
        {
             // Arrange
            var oauthId = $"{Guid.NewGuid()} @ OAuth";
            var emailAddress = $"user@{Guid.NewGuid()}.com";
            const int USER_ID = 8888;
            
            var userRepository = new Mock<IUserRepository>();
            userRepository.Setup(u => u.GetUserId(oauthId)).Returns(USER_ID);

            var buildingsRepository = new Mock<IBuildingsRepository>();
            var resourcesRepository = new Mock<IResourcesRepository>();
            
            int expectedCrystals = 28;
            resourcesRepository.Setup(r => r.GetResources(USER_ID)).Returns(new Dictionary<ResourceType, int>() {
                { ResourceType.Crystals, expectedCrystals },
            });

            var controller = new HomeController(new Mock<ILogger<HomeController>>().Object, userRepository.Object, buildingsRepository.Object, resourcesRepository.Object);
            controller.MockUserClaims(oauthId, emailAddress);

            // Act
            controller.Dashboard();

            // Assert
            Dictionary<ResourceType, int> actual = controller.ViewBag.Resources;

            Assert.AreEqual(actual[ResourceType.Crystals], expectedCrystals);
        }
    }
}