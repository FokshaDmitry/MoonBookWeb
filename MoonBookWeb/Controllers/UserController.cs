using Microsoft.AspNetCore.Mvc;
using MoonBookWeb.Services;

namespace MoonBookWeb.Controllers
{
    public class UserController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ISessionLogin _sessionLogin;
        private readonly AddDbContext _context;

        public UserController(ILogger<HomeController> logger, ISessionLogin sessionLogin, AddDbContext context)
        {
            _logger = logger;
            _sessionLogin = sessionLogin;
            _context = context;
        }

        public IActionResult Index()
        {
            if (_sessionLogin.user != null)
            {
                ViewData["AuthUser"] = _sessionLogin?.user;
                ViewData["PostUser"] = _context.Posts.Where(p => p.IdUser == _sessionLogin.user.Id).OrderByDescending(p => p.Date);
                return View();
            }
            return Redirect("/Login/Index");
        }
        public IActionResult FreandPage()
        {
            ViewData["AuthUser"] = _sessionLogin?.user;
            return View();
        }
    }
}
