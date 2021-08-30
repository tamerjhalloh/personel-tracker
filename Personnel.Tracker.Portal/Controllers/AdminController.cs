using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using Personnel.Tracker.Model.Base;
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

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);

            if (CurrentPersonnel.Role != PersonnelRole.Admin.ToString())
            {
                filterContext.Result = new RedirectToRouteResult(new Microsoft.AspNetCore.Routing.RouteValueDictionary
                    {
                        {"controller", "home"},
                        {"action", "index"}
                    });
            }

          
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
