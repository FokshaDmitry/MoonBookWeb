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
                postUser.TextPost = postUser.TextPost?.Replace($">{postUser.TitlePost}<", "");
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
                    postUser.ImagePost.CopyToAsync(new FileStream("./wwwroot/img_post/" + posts.Image, FileMode.Create));        
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
        [HttpPut]
        public object Reaction([FromForm]Reactions reactions)
        {
            var qerty = _context.Reactions.Where(r => r.IdPost == reactions.IdPost).Join(_context.Posts, r => r.IdPost, p => p.Id, (r, p) => new { Ract = r, Post = p });
            foreach (var reac in qerty)
            {
                if (reac.Ract.IdUser == _sessionLogin.user.Id)
                {
                    if (reac.Ract.Reaction == reactions.Reaction && reac.Ract.Reaction == 1)
                    {
                        reac.Ract.Reaction = 0;
                        reac.Post.Like--;
                        _context.Posts.Update(reac.Post);
                        _context.Reactions.Update(reac.Ract);
                    }
                    else if (reac.Ract.Reaction == reactions.Reaction && reac.Ract.Reaction == 2)
                    {
                        reac.Ract.Reaction = 0;
                        reac.Post.Dislike--;
                        _context.Posts.Update(reac.Post);
                        _context.Reactions.Update(reac.Ract);
                    }
                    
                    else if (reac.Ract.Reaction != reactions.Reaction && reactions.Reaction == 1)
                    {
                        reac.Ract.Reaction = 1;
                        reac.Post.Like++;
                        if (reac.Post.Dislike != 0) reac.Post.Dislike--;
                        _context.Posts.Update(reac.Post);
                        _context.Reactions.Update(reac.Ract);
                    }
                    else if (reac.Ract.Reaction != reactions.Reaction && reactions.Reaction == 2)
                    {
                        reac.Ract.Reaction = 2;
                        reac.Post.Dislike++;
                        if (reac.Post.Like != 0) reac.Post.Like--;
                        _context.Posts.Update(reac.Post);
                        _context.Reactions.Update(reac.Ract);
                    }
                }
                else
                {
                    if (reactions.Reaction == 1) reac.Post.Like++;
                    else reac.Post.Dislike++;

                    _context.Posts.Update(reac.Post);
                    _context.Reactions.Add(new Reactions { Id = Guid.NewGuid(), IdPost = reactions.IdPost, IdUser = _sessionLogin.user.Id, Reaction = reactions.Reaction });
                }
            }
            if (qerty.Count() == 0)
            {
                var q = _context.Posts.Where(p => p.Id == reactions.IdPost).FirstOrDefault();
                if (reactions.Reaction == 1) q.Like++;
                else q.Dislike++;

                _context.Reactions.Add(new Reactions { Id = Guid.NewGuid(), IdPost = reactions.IdPost, IdUser = _sessionLogin.user.Id, Reaction = reactions.Reaction });
                _context.Posts.Update(q);
            }
            _context.SaveChanges();
            return new { status = "Ok", reactLike = _context.Posts.Where(p => p.Id == reactions.IdPost).FirstOrDefault().Like, reactDislike = _context.Posts.Where(p => p.Id == reactions.IdPost).FirstOrDefault().Dislike };
        }
    }
}
