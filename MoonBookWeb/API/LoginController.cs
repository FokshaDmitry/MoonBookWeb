using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MoonBookWeb.Services;

namespace MoonBookWeb.API
{
    [Route("api/login")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly AddDbContext _context;
        private readonly Services.Hesher _hesher;

        public LoginController(AddDbContext context, Hesher hesher)
        {
            _context = context;
            _hesher = hesher;
        }

        [HttpGet]
        public object Get(string login, string password)
        {
            if (string.IsNullOrEmpty(login))
            {
                HttpContext.Response.StatusCode = 409;
                return "Conflict: login required";
            }
            if (string.IsNullOrEmpty(password))
            {
                HttpContext.Response.StatusCode = 409;
                return "Conflict: password required";
            }
            User user = _context.Users.Where(u => u.Login == login).FirstOrDefault();

            if (user == null)
            {
                HttpContext.Response.StatusCode = 401;
                return "Unauthorized: credentials rejected";
            }

            String PassHash = _hesher.Hesh(password + user.PassSalt);

            if (PassHash != user.Password)
            {
                HttpContext.Response.StatusCode = 401;
                return "Unauthorized: credentials invalid";
            }
            return user;
        }
    }
}
