using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PhotoBrowser
{
    public class SessionFilterAttribute : ActionFilterAttribute
    {
        internal const string SESSION_COOKIE = "PhotoBrowser.Session";

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (string.IsNullOrWhiteSpace(GetSessionId(context.HttpContext)))
            {
                context.Result = new RedirectResult("/session", false, false);
            }

            base.OnActionExecuting(context);
        }

        public static string GetSessionId(HttpContext context)
        {
            return context.Request.Cookies[SESSION_COOKIE];
        }
    }
}