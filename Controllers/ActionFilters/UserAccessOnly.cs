using DotNetCoreCalendar.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection; // <-- needed for GetRequiredService
using System;

namespace DotNetCoreCalendar.Controllers.ActionFilters
{
    public class UserAccessOnly : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (context.RouteData.Values.ContainsKey("id"))
            {
                // resolve your DAL from DI
                var dal = context.HttpContext.RequestServices.GetRequiredService<IDAL>();

                var idStr = context.RouteData.Values["id"]?.ToString();
                if (int.TryParse(idStr, out var id))
                {
                    var username = context.HttpContext.User?.Identity?.Name;
                    var myevent = dal.GetEvent(id);

                    if (myevent?.User != null && username != null &&
                        !string.Equals(myevent.User.UserName, username, StringComparison.OrdinalIgnoreCase))
                    {
                        context.Result = new RedirectToRouteResult(
                            new RouteValueDictionary(new { controller = "Home", action = "NotFound" }));
                    }
                }
            }

            base.OnActionExecuting(context);
        }
    }
}
