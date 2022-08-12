using Microsoft.AspNetCore.Mvc;
using MoonBookWeb.Services;

namespace MoonBookWeb.Controllers
{
    public class UserController : Controller
    {
        private readonly ISessionLogin _sessionLogin;
        private readonly AddDbContext _context;

        public UserController(ISessionLogin sessionLogin, AddDbContext context)
        {
            _sessionLogin = sessionLogin;
            _context = context;
        }

        public IActionResult Index()
        {
            if (_sessionLogin.user != null)
            {
                ViewData["AuthUser"] = _sessionLogin?.user;
                return View();
            }
            return Redirect("/Login/Index");
        }
        public IActionResult FreandPage()
        {
            if (_sessionLogin.user != null)
            {
                List<User> freands = new List<User>();

                foreach (var user in _sessionLogin.userFreands)
                {
                    var q = _context.Users.Find(user);
                    q.Password = "*";
                    q.PassSalt = "*";
                    freands.Add(q);
                }
                ViewData["Freands"] = freands;
                ViewData["AuthUser"] = _sessionLogin?.user;
                return View();
            }
            else
            {
                return Redirect("/Login/Index");
            }
        }
    }
}
