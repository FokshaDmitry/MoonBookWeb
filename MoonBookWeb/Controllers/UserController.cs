using Microsoft.AspNetCore.Mvc;
using MoonBookWeb.Services;

namespace MoonBookWeb.Controllers
{
    public class UserController : Controller
    {
        private readonly ISessionLogin _sessionLogin;

        public UserController(ISessionLogin sessionLogin)
        {
            _sessionLogin = sessionLogin;
        }
        //User Page
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
                ViewData["Freands"] = "Ok";
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
