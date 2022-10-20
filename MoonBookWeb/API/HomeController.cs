using Microsoft.AspNetCore.Mvc;
using MoonBookWeb.Services;
using System.Xml.Linq;

namespace MoonBookWeb.API
{
    [Route("api/home")]
    [ApiController]
    public class HomeController : ControllerBase
    {
        private readonly AddDbContext _context;

        private readonly ISessionLogin _sessionLogin;

        public HomeController(AddDbContext context, ISessionLogin sessionLogin)
        {
            _sessionLogin = sessionLogin;
            _context = context;
        }
        //Get All Post
        [HttpGet]
        public object Get()
        {
            var comments = _context.Comments.Where(c => c.Delete == Guid.Empty).Join(_context.Users, c => c.idUser, u => u.Id, (c, u) => new { Comment = c, User = u }).ToList();
            foreach (var comment in comments)
            {
                if (comment.Comment.Answer != Guid.Empty)
                {
                    var user = _context.Users.Find(comment.Comment.Answer);
                    comment.Comment.Text = $"<a href='../User/FreandPage?{user?.Login}'>{user?.Name} {user?.Surname}</a>, {comment.Comment.Text}";
                }
            }
            var post = _context.Posts.Where(p => p.Delete == Guid.Empty).ToList().Join(_context.Users, p => p.IdUser, u => u.Id, (p, u) => new { post = p, user = u }).OrderByDescending(p => p.post.Date).GroupJoin(comments, p => p.post.Id, c => c.Comment.idPost, (p, c) => new { Post = p, Comment = c });
            if(_sessionLogin.user == null)
            {
                return new { status = "Ok", message = post };
            }
            else
            {
                return new { status = "Ok", message = post, user = _sessionLogin.user.Id };
            }
        }
    }
}