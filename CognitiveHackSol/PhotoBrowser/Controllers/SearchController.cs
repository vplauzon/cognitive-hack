using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace PhotoBrowser.Controllers
{
    [SessionFilter]
    public class SearchController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}