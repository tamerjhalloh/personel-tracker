using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Personnel.Tracker.Portal.Models;

namespace Personnel.Tracker.Portal.Controllers
{
    public class MemberController : BaseController
    {
        private readonly ILogger<MemberController> _logger;

        public MemberController(ILogger<MemberController> logger)
        {
            _logger = logger;
        }

        public IActionResult Login()
        {
            return View();
        } 
    }
}
