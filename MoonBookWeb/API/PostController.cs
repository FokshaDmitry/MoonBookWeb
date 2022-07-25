using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MoonBookWeb.Services;

namespace MoonBookWeb.API
{
    [Route("api/post")]
    [ApiController]
    public class PostController : ControllerBase
    {
        private readonly ISessionLogin _sessionLogin; 
        private readonly AddDbContext _context;
        public PostController(ISessionLogin sessionLogin, AddDbContext context)
        {
            _sessionLogin = sessionLogin;
            _context = context;
        }
        [HttpPost]
        public object Post([FromForm]Models.PostUserModel postUser)
        {
            if(_sessionLogin.user != null)
            {
                Posts posts = new Posts();
                bool isValid = false;
                postUser.TextPost?.Replace($">{postUser.TitlePost}<", "");
                if (!String.IsNullOrEmpty(postUser.TextPost))
                {
                    posts.Text = postUser.TextPost;
                    isValid = true;
                }
                if (!String.IsNullOrEmpty(postUser.TitlePost))
                {
                    isValid = true;
                    posts.Title = postUser.TitlePost;
                }
                if (postUser.ImagePost != null)
                {
                    
                    isValid = true;
                    posts.Image = Guid.NewGuid().ToString() + Path.GetExtension(postUser.ImagePost.FileName);
                    postUser.ImagePost.CopyToAsync(
                        new FileStream(
                            "./wwwroot/img_post/" + posts.Image,
                            FileMode.Create));
                    
                }
                if (isValid)
                {
                    posts.Id = Guid.NewGuid();
                    posts.Date = DateTime.Now;
                    posts.IdUser = _sessionLogin.user.Id;
                    posts.Like = 0;
                    posts.Dislike = 0;
                    _context.Posts.Add(posts);
                    _context.SaveChanges();
                    return new { status = "Ok", message = postUser.TextPost };
                }
                else
                {
                    return new { status = "Error", message = "Post Empty" };
                }
            }
            else
            {
                return new { status = "Error", message = "User don't find" };
            }
            
        }
        
    }
}
