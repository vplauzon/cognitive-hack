using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PhotoBrowser.Models;

namespace PhotoBrowser.Controllers
{
    public class SessionController : Controller
    {
        public IActionResult Index()
        {
            HttpContext.Response.Cookies.Delete(SessionFilterAttribute.SESSION_COOKIE);

            return View();
        }

        [HttpPost]
        public IActionResult Index(SessionViewModel model)
        {
            HttpContext.Response.Cookies.Append(SessionFilterAttribute.SESSION_COOKIE, model.SessionID);

            return RedirectToAction(null, "Home");
        }
    }
}