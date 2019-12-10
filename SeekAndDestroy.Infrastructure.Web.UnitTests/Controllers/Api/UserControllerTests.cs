using System;
using System.Collections.Generic;
using System.Security.Claims;
using Moq;
using NUnit.Framework;
using SeekAndDestroy.Core.DataAccess;
using SeekAndDestroy.Infrastructure.Web.Api.Controllers;

namespace SeekAndDestroy.Infrastructure.Web.UnitTests.Controllers
{
    [TestFixture]
    public class UserControllerTests
    {
        [Test]
        public void GetOAuth2IdGetsIdFromClaims()
        {
            // Arrange
            var expectedOauthId = $"{Guid.NewGuid()} @ OAuth";
            var userName = $"user@{Guid.NewGuid()}.com";
            
            var identity = new ClaimsIdentity(new List<Claim>() {
                new Claim(ControllerExtensions.OAUTH_ID_CLAIM, expectedOauthId),
                new Claim(ControllerExtensions.EMAIL_ADDRESS_CLAIM, userName),
            }, ControllerExtensions.CLAIM_TYPE);

            var userRepository = new Mock<IUserRepository>();
            var buildingsRepository = new Mock<IBuildingsRepository>();
            var resourcesRepository = new Mock<IResourcesRepository>();
            
            var controller = new UserController(identity, userRepository.Object, buildingsRepository.Object, resourcesRepository.Object);

            // Act
            var actual = controller.GetOAuth2Id();

            // Assert
            Assert.That(actual, Is.EqualTo(expectedOauthId));
        }

        [Test]
        public void GetUserIdGetsUserInDatabaseMatchingOauthIdClaim()
        {
            // Arrange
            var oauthId = $"{Guid.NewGuid()} @ OAuth";
            var emailAddress = $"user@{Guid.NewGuid()}.com";
            const int EXPECTED_USER_ID = 1902;
            
            var identity = new ClaimsIdentity(new List<Claim>() {
                new Claim(ControllerExtensions.OAUTH_ID_CLAIM, oauthId),
                new Claim(ControllerExtensions.EMAIL_ADDRESS_CLAIM, emailAddress),
            }, ControllerExtensions.CLAIM_TYPE);

            var userRepository = new Mock<IUserRepository>();
            userRepository.Setup(u => u.GetUserId(oauthId)).Returns(EXPECTED_USER_ID);

            var buildingsRepository = new Mock<IBuildingsRepository>();
            var resourcesRepository = new Mock<IResourcesRepository>();
            
            var controller = new UserController(identity, userRepository.Object, buildingsRepository.Object, resourcesRepository.Object);

            // Act
            var actual = controller.GetUserId();

            // Assert
            Assert.That(actual, Is.EqualTo(EXPECTED_USER_ID));
        }

        [Test]
        public void CreateNewUserCreatesUserResourcesAndBuildingsinDatabase()
        {
            // Arrange
            var oauthId = $"{Guid.NewGuid()} @ OAuth";
            var emailAddress = $"user@{Guid.NewGuid()}.com";
            
            var identity = new ClaimsIdentity(new List<Claim>() {
                new Claim(ControllerExtensions.OAUTH_ID_CLAIM, oauthId),
                new Claim(ControllerExtensions.EMAIL_ADDRESS_CLAIM, emailAddress),
            }, ControllerExtensions.CLAIM_TYPE);

            var userRepository = new Mock<IUserRepository>();
            var buildingsRepository = new Mock<IBuildingsRepository>();
            var resourcesRepository = new Mock<IResourcesRepository>();
            
            var controller = new UserController(identity, userRepository.Object, buildingsRepository.Object, resourcesRepository.Object);

            // Act
            controller.CreateNewUser();

            // Assert
            userRepository.Verify(u => u.CreateUser(oauthId, emailAddress));
            buildingsRepository.Verify(b => b.InitializeForUser(It.IsAny<int>()));
            resourcesRepository.Verify(r => r.InitializeForUser(It.IsAny<int>()));
        }
    }
}