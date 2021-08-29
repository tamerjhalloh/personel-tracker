using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Personnel.Tracker.Portal.Models;

namespace Personnel.Tracker.Portal.Controllers
{
    public class AdminController : AuthController
    {
        private readonly ILogger<AdminController> _logger;

        public AdminController(ILogger<AdminController> logger)
        {
            _logger = logger;
        }

        public IActionResult Personnel()
        {
            return View();
        }

        public IActionResult Attendance()
        {
            return View();
        }
    }
}
