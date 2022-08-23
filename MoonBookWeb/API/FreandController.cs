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
                return new {status = "Ok", message = freands };
            }
            return new {status = "Ok", message = "You don't have freands" };
        }
        [HttpGet("{Message}")]
        public object Posts(string message)
        {
            if (String.IsNullOrEmpty(message))
            {
                return new { status = "Error", message = "User is empty" };
            }
            if (message == "Post")
            {
                var freands = _context.Subscriptions.Where(s => s.IdUser == _sessionLogin.user.Id).Join(_context.Users, s => s.IdFreand, u => u.Id, (s, u) => new { Sub = s, User = u }).Select(s => s.User);
                if (freands.Any())
                {
                    var postFreand = _context.Posts.Join(freands, p => p.IdUser, u => u.Id, (p, u) => new { Post = p, User = u });
                    if (postFreand.Any())
                    {
                        return new { status = "Ok", message = postFreand };
                    }
                }
                return new { status = "Error", message = "Dont find Posts" };
            }
            else
            {
                Guid Id = Guid.Parse(message);
                var freandsPost = _context.Posts.Where(p => p.IdUser == Id).ToList().Join(_context.Users, p => p.IdUser, u => u.Id, (p, u) => new { User = u, Post = p }).OrderByDescending(u => u.Post.Date).GroupJoin(_context.Comments, p => p.Post.Id, c => c.idPost, (p, c) => new { Post = p, Comment = c }); ;
                if (freandsPost.Any())
                {
                    return new { status = "Ok", message = freandsPost };
                }
                return new { status = "Error", message = "Dont find Posts" };
            }
        }
        [HttpPut]
        public object Search([FromForm]string Name)
        {
            if (!String.IsNullOrEmpty(Name))
            {
                Name = Name.Replace(" ", "").ToLower();
                var sub = _context.Subscriptions.Where(s => s.IdUser == _sessionLogin.user.Id);
                var users = _context.Users.Where(s => s.Name.ToLower() + s.Surname.ToLower() == Name).GroupJoin(sub, u => u.Id, s => s.IdFreand, (u, s) => new { User = u, Sub = s});
                if (users.Select(u => u.User).Count() > 0)
                {
                    return new { status = "Ok", message = users};
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
            bool b = true;
            if(String.IsNullOrEmpty(id))
            {
                return new { status = "Error", message = "User is empty" };
            }
            Guid Id = Guid.Parse(id);
            var user = _context.Subscriptions.Where(s => s.IdUser == _sessionLogin.user.Id).Where(s => s.IdFreand == Id).FirstOrDefault();
            if(user == null)
            {
                _context.Subscriptions.Add(new Subscriptions { Id = Guid.NewGuid(), IdFreand = Id, IdUser = _sessionLogin.user.Id });
                b = true;
            }
            else
            {
                _context.Subscriptions.Remove(user);
                b = false;
            }
            _context.SaveChanges();
            return new { status = "Ok", message = b };
        }
    }
}
