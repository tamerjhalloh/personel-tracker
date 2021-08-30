using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Personnel.Tracker.Portal.Models;

namespace Personnel.Tracker.Portal.Controllers
{
    public class MemberController : BaseController
    {
        private readonly ILogger<MemberController> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public MemberController(ILogger<MemberController> logger, IHttpContextAccessor httpContextAccessor)
        {
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
        }

        public IActionResult Login()
        {
            return View();
        }



        public async Task<IActionResult> Signout()
        {
            await HttpContext.SignOutAsync();
            HttpContext.Items["user"] = null;
            _httpContextAccessor.HttpContext.Response.Cookies.Delete("token");
            _httpContextAccessor.HttpContext.Response.Cookies.Delete("refreshToken"); 
            return Redirect("/member/login");
        }
    }
}
