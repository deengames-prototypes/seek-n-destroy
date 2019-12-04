using Moq;
using NUnit.Framework;
using SeekAndDestroy.Infrastructure.Web.Controllers;

namespace SeekAndDestroy.Infrastructure.Web.Tests.Controllers
{
    [TestFixture]
    public class HomeControllerTests
    {
        [Test]
        public void HelloWorld()
        {
            Assert.That(2, Is.EqualTo(2));
            var test = new Mock<HomeController>();
        }
    }
}