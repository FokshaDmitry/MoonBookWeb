using Microsoft.AspNetCore.Mvc;
using MoonBookWeb.Services;

namespace MoonBookWeb.API
{
    [Route("api/user")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly ISessionLogin _sessionLogin;
        private readonly AddDbContext _context;

        public UserController(ISessionLogin sessionLogin, AddDbContext context)
        {
            _sessionLogin = sessionLogin;
            _context = context;
        }

        [HttpGet]
        public object OnlineFreands()
        {
            var freand = _context.Subscriptions.Where(s => s.IdUser == _sessionLogin.user.Id).Join(_context.Users, s => s.IdFreand, u => u.Id, (s, u) => new { Sub = s, User = u }).Select(u => u.User).Where(u => u.Online == true);
            return new { status = "Ok", message = freand };
        }
    }
}
