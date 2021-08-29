using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Personnel.Tracker.Portal.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Personnel.Tracker.Portal.Middlewares
{
    public class UserHanlerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<UserHanlerMiddleware> _logger;

        public UserHanlerMiddleware(RequestDelegate next, ILogger<UserHanlerMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            try
            {
                var userIdentity = UserIdentityHelper.GetUserFromIdentity(httpContext);
                httpContext.Items["user"] = userIdentity;

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception while handlling user");
            }
            await _next(httpContext);
        }
    }

    public static class UserHanlerMiddlewareExtension
    {
        public static IApplicationBuilder UseUserHandler(this IApplicationBuilder applicationBuilder)
        {
            return applicationBuilder.UseMiddleware<UserHanlerMiddleware>();
        }
    }
}
