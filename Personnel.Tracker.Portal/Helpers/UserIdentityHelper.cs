using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Personnel.Tracker.Portal.Models;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Personnel.Tracker.Portal.Helpers
{
    public class UserIdentityHelper
    {
        public static async Task SetIdentity(HttpContext context, UserIdentity identity)
        {
            List<Claim> userClaims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, identity.Id.ToString()),
                new Claim(ClaimTypes.Email, identity.Email),
                new Claim(ClaimTypes.GivenName, $"{identity.Name} {identity.Surname}")
            };
            var userIdentity = new ClaimsIdentity(userClaims, "Personnel");
            ClaimsPrincipal principal = new ClaimsPrincipal(userIdentity);
            await context.SignInAsync(principal, new AuthenticationProperties { ExpiresUtc = DateTime.Now.AddMinutes(60) });
            context.Response.Cookies.Append("_refreshtoken", identity.RefreshToken);
            context.Response.Cookies.Append("_accesstoken", identity.Token);
        }

        public static Model.Personnel.Personnel GetUserFromIdentity(HttpContext context)
        {
            try
            {
                if (context.Items["user"] != null)
                    return context.Items["user"] as Model.Personnel.Personnel;
            }
            catch (Exception ex)
            {

            }
            return null;
        }
    }
}
