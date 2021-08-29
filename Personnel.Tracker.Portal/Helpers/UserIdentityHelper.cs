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
                new Claim(ClaimTypes.NameIdentifier, identity.Id.ToString()),
                new Claim(ClaimTypes.Email, identity.Email),
                new Claim(ClaimTypes.Name,identity.Name),
                new Claim(ClaimTypes.GivenName,identity.Surname),
                new Claim(ClaimTypes.Role, identity.Role)
            };
            var userIdentity = new ClaimsIdentity(userClaims, "Personnel");
            ClaimsPrincipal principal = new ClaimsPrincipal(userIdentity);
            await context.SignInAsync(principal, new AuthenticationProperties { ExpiresUtc = DateTime.Now.AddMinutes(60) });
            context.Response.Cookies.Append("_refreshtoken", identity.RefreshToken);
            context.Response.Cookies.Append("_accesstoken", identity.Token);
        }

        public static UserIdentity GetUserFromIdentity(HttpContext context)
        {
            try
            {
                var currentUser = context.User;

                UserIdentity user = new UserIdentity
                {

                    Id = Guid.Parse(currentUser.FindFirstValue(ClaimTypes.NameIdentifier)),
                    Name = currentUser.FindFirstValue(ClaimTypes.Name),
                    Surname = currentUser.FindFirstValue(ClaimTypes.GivenName),
                    Email = currentUser.FindFirstValue(ClaimTypes.Email),
                    Role = currentUser.FindFirstValue(ClaimTypes.Role)
                };

                return user;
            }
            catch (Exception ex)
            {

            }

            return null;
        }
    }
}
