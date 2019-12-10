using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace SeekAndDestroy.Infrastructure.Web.UnitTests.Controllers
{
    public static class ControllerExtensions
    {
        private const string OAUTH_ID_CLAIM = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier";
        private const string EMAIL_ADDRESS_CLAIM = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress";

        public static void MockUserClaims(this Controller controller, string oauthId, string emailAddress)
        {
            // Spoof ID
            var identities = new List<ClaimsIdentity>()
            {
                new ClaimsIdentity(new List<Claim>() {
                    new Claim(OAUTH_ID_CLAIM, oauthId),
                    new Claim(EMAIL_ADDRESS_CLAIM, emailAddress),
                }, "AuthenticationTypes.Federation"),
            };

            var httpContext = new DefaultHttpContext()
            {
                User = new System.Security.Claims.ClaimsPrincipal(identities)
            };

            // Modified from https://stackoverflow.com/a/41400246/8641842
            controller.ControllerContext = new ControllerContext();
            controller.ControllerContext.HttpContext = httpContext;
        }
    }
}