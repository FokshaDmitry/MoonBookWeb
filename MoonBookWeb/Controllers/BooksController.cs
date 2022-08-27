using Microsoft.AspNetCore.Mvc;
using MoonBookWeb.Services;

namespace MoonBookWeb.Controllers
{
    public class BooksController : Controller
    {
        private readonly ISessionLogin _sessionLogin;

        public BooksController(ISessionLogin sessionLogin)
        {
            _sessionLogin = sessionLogin;
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
    }
}
