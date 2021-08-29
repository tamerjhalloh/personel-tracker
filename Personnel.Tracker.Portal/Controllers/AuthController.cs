using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Personnel.Tracker.Common;
using Personnel.Tracker.Portal.Models;
using System.Net;

namespace Personnel.Tracker.Portal.Controllers
{
    public class AuthController : BaseController
    {

        public UserIdentity CurrentPersonnel { get; set; }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);
            var isAjax = Request.Headers["X-Requested-With"] == "XMLHttpRequest";

            var path = $"{filterContext.HttpContext.Request.RouteValues["controller"]}/{filterContext.HttpContext.Request.RouteValues["action"]}";


            if (filterContext.HttpContext.Items["user"] == null && !path.EqualsInsensitive("Api/SignIn"))
            {
                filterContext.HttpContext.SignOutAsync();

                if (isAjax)
                {
                    filterContext.Result = Forbid();
                }
                else
                    filterContext.Result = new RedirectToRouteResult(new Microsoft.AspNetCore.Routing.RouteValueDictionary
                    {
                        {"controller", "member"},
                        {"action", "login"}
                    }
                    );
                filterContext.HttpContext.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                return;
            }



            CurrentPersonnel = filterContext.HttpContext.Items["user"] as UserIdentity;
            ViewBag.ViewPersonnel = CurrentPersonnel;
        }


    }
}
