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
        private readonly AddDbContext _context;
        public HomeController(ILogger<HomeController> logger, ISessionLogin sessionLogin, AddDbContext context)
        {
            _context = context;
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
            ViewData["AuthUser"] = _sessionLogin?.user;
            return View();
        }
        public IActionResult UserPage()
        {
            if (_sessionLogin.user != null)
            {
                ViewData["AuthUser"] = _sessionLogin?.user;
                ViewData["PostUser"] = _context.Posts.Where(p => p.IdUser == _sessionLogin.user.Id).OrderByDescending(p => p.Date);
                return View();
            }
            return Redirect("/Login/Index");
        }
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}