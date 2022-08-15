using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MoonBookWeb.Services;

namespace MoonBookWeb.API
{
    [Route("api/freand")]
    [ApiController]
    public class FreandController : ControllerBase
    {
        private readonly AddDbContext _context;
        private readonly ISessionLogin _sessionLogin;

        public FreandController(AddDbContext context, ISessionLogin sessionLogin)
        {
            _sessionLogin = sessionLogin;
            _context = context;
        }
        [HttpGet]
        public object Get()
        {
            var freands = _context.Subscriptions.Where(s => s.IdUser == _sessionLogin.user.Id);
            if(freands.Any())
            {
                List<User> users = new List<User>();
                foreach (var frend in freands)
                {
                    var user = _context.Users.Find(frend.IdFreand);
                    user.PassSalt = "*";
                    user.Password = "*";
                    users.Add(user);
                }
                return new {status = "Ok", message = users };
            }
            return new {status = "Ok", message = "You don't have freands" };
        }
        [HttpPut]
        public object Search([FromForm]string Name)
        {
            if (!String.IsNullOrEmpty(Name))
            {
                Name = Name.Replace(" ", "").ToLower();
                var name = _context.Users.Where(s => s.Name.ToLower() + s.Surname.ToLower() == Name);
                var sub = _context.Subscriptions.Where(s => s.IdUser == _sessionLogin.user.Id).Select(s => s.IdFreand);
                if (name.Any())
                {
                    List<User> users = new List<User>();
                    foreach (var user in name)
                    {
                        user.Password = "*";
                        user.PassSalt = "*";
                        users.Add(user);
                    }
                    return new { status = "Ok", message = users, sub = sub};
                }
                else
                {
                    return new { status = "Undefinded", message = "User dont find" };
                }
            }
            return new { status = "Error", message = "No data to request" };
        }
    }
}
