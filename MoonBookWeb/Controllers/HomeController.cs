using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using MoonBookWeb.Models;
using MoonBookWeb.Services;
using System.Diagnostics;

namespace MoonBookWeb.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ISessionLogin _sessionLogin;
        public HomeController(ILogger<HomeController> logger, ISessionLogin sessionLogin)
        {
            _logger = logger;
            _sessionLogin = sessionLogin;
        }
        public IActionResult Index()
        {
            ViewData["AuthUser"] = _sessionLogin?.user;
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }
        public IActionResult UserPage()
        {
            ViewData["AuthUser"] = _sessionLogin?.user;
            return View();
        }
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}