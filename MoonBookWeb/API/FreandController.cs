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
            var freands = _context.Subscriptions.Where(s => s.IdUser == _sessionLogin.user.Id).Join(_context.Users, s => s.IdFreand, u => u.Id, (s, u) => new { Sub = s, User = u }).Select(s => s.User);
            if (freands.Any())
            {
                foreach(var freand in freands)
                {
                    freand.PassSalt = "*";
                    freand.Password ="*";
                }
                return new {status = "Ok", message = freands };
            }
            return new {status = "Ok", message = "You don't have freands" };
        }
        [HttpGet("{Posts}")]
        public object Posts()
        {
            var postFreand = _context.Subscriptions.Where(s => s.IdUser == _sessionLogin.user.Id).Join(_context.Posts, s => s.IdFreand, p => p.IdUser, (s, p) => new { Sub = s, Post = p }).Select(p => p.Post).OrderByDescending(p => p.Date);
            if(postFreand.Any())
            {
                return new { status = "Ok", message = postFreand };
            }
            return new { status = "Error", message = "Dont find Posts" };
        }
        [HttpPut]
        public object Search([FromForm]string Name)
        {
            if (!String.IsNullOrEmpty(Name))
            {
                Name = Name.Replace(" ", "").ToLower();
                var users = _context.Users.Where(s => s.Name.ToLower() + s.Surname.ToLower() == Name);
                var sub = _context.Subscriptions.Where(s => s.IdUser == _sessionLogin.user.Id).Select(s => s.IdFreand);
                if (users.Any())
                {
                    foreach (var user in users)
                    {
                        user.Password = "*";
                        user.PassSalt = "*";
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
        [HttpPut("{id}")]
        public object Follow(string id)
        {
            if(String.IsNullOrEmpty(id))
            {
                return new { status = "Error", message = "User is empty" };
            }
            Guid Id = Guid.Parse(id);
            var user = _context.Subscriptions.Where(s => s.IdUser == _sessionLogin.user.Id).Where(s => s.IdFreand == Id).FirstOrDefault();
            if(user == null)
            {
                _context.Subscriptions.Add(new Subscriptions { Id = Guid.NewGuid(), IdFreand = Id, IdUser = _sessionLogin.user.Id });
            }
            else
            {
                _context.Subscriptions.Remove(user);
            }
            _context.SaveChanges();
            return new { status = "Ok" };
        }
    }
}
