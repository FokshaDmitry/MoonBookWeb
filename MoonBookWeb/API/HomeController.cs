using Microsoft.AspNetCore.Mvc;

namespace MoonBookWeb.API
{
    [Route("api/home")]
    [ApiController]
    public class HomeController : ControllerBase
    {
        private readonly AddDbContext _context;

        public HomeController(AddDbContext context)
        {
            _context = context;
        }
        //Get All Post
        [HttpGet]
        public object Get()
        {
            var comment = _context.Comments.Where(c => c.Delete == Guid.Empty).Join(_context.Users, c => c.idUser, u => u.Id, (c, u) => new { Comment = c, User = u });
            var post = _context.Posts.Where(p => p.Delete == Guid.Empty).ToList().Join(_context.Users, p => p.IdUser, u => u.Id, (p, u) => new { post = p, user = u }).OrderByDescending(p => p.post.Date).GroupJoin(comment, p => p.post.Id, c => c.Comment.idPost, (p, c) => new { Post = p, Comment = c });
            return new { status = "Ok", message = post };
        }
    }
}
