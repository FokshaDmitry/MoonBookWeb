using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using MoonBookWeb.Models;
using MoonBookWeb.Services;
using System.Linq;

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
        #region Get
        //Find post current User
        [HttpGet("{Message}")]
        public object Get(string message)
        {
            if (String.IsNullOrEmpty(message))
            {
                return new { status = "Error", message = "User is empty" };
            }
            var comments = _context.Comments.Where(c => c.Delete == Guid.Empty).Join(_context.Users, c => c.idUser, u => u.Id, (c, u) => new { Comment = c, User = u }).ToList().GroupJoin(_context.Users, c=> c.Comment.Answer, u => u.Id, (c, u) => new {Comment = c, UserAnswer = u }).OrderBy(c => c.Comment.Comment.Date);
            if (message == "user")
            {
                var post = _context.Posts.Where(p => p.IdUser == _sessionLogin.user.Id)
                                     .Where(p => p.Delete == Guid.Empty).ToList()
                                     .Join(_context.Users, p => p.IdUser, u => u.Id, (p, u) => new { post = p, user = u })
                                     .OrderByDescending(p => p.post.Date)
                                     .GroupJoin(comments, p => p.post.Id, c => c.Comment.Comment.idPost, (p, c) => new { Post = p, Comment = c })
                                     .GroupJoin(_context.Reactions, p => p.Post.post.Id, r => r.IdPost, (p, r) => new { post = p, Like = r.Where(l => l.Reaction == 1).Count(), Dislike = r.Where(l => l.Reaction == 2).Count() });
                return new { status = "Ok", message = post, user = _sessionLogin.user.Id };

            }
            if (message == "home")
            {
                var post = _context.Posts.Where(p => p.Delete == Guid.Empty).ToList().Join(_context.Users, p => p.IdUser, u => u.Id, (p, u) => new { post = p, user = u }).OrderByDescending(p => p.post.Date).GroupJoin(comments, p => p.post.Id, c => c.Comment.Comment.idPost, (p, c) => new { Post = p, Comment = c }).GroupJoin(_context.Reactions, p => p.Post.post.Id, r => r.IdPost, (p, r) => new { post = p, Like = r.Where(l => l.Reaction == 1).Count(), Dislike = r.Where(l => l.Reaction == 2).Count() });
                if (_sessionLogin.user == null)
                {
                    return new { status = "Ok", message = post };
                }
                else
                {
                    return new { status = "Ok", message = post, user = _sessionLogin.user.Id };
                }
            }
            //Get all post of all freands
            if (message == "freand")
            {
                var freands = _context.Subscriptions.Where(s => s.IdUser == _sessionLogin.user.Id).Join(_context.Users, s => s.IdFreand, u => u.Id, (s, u) => new { Sub = s, User = u }).Select(s => s.User);
                if (freands != null)
                {
                    var postFreand = _context.Posts.ToList().Join(freands, p => p.IdUser, u => u.Id, (p, u) => new { Post = p, User = u }).OrderByDescending(u => u.Post.Date).GroupJoin(comments, p => p.Post.Id, c => c.Comment.Comment.idPost, (p, c) => new { Post = p, Comment = c }).GroupJoin(_context.Reactions, p => p.Post.Post.Id, r => r.IdPost, (p, r) => new { post = p, Like = r.Where(l => l.Reaction == 1).Count(), Dislike = r.Where(l => l.Reaction == 2).Count() });
                    if (postFreand != null)
                    {
                        return new { status = "Ok", message = postFreand, user = _sessionLogin.user.Id };
                    }
                }
                return new { status = "Error", message = "Dont find Posts" };
            }
            //Get choose frend post
            else
            {
                var user = _context.Users.Where(u => u.Login == message).FirstOrDefault();
                if (user != null)
                {
                    var freandsPost = _context.Posts.Where(p => p.IdUser == user.Id).ToList().Join(_context.Users, p => p.IdUser, u => u.Id, (p, u) => new { User = u, Post = p }).OrderByDescending(u => u.Post.Date).GroupJoin(comments, p => p.Post.Id, c => c.Comment.Comment.idPost, (p, c) => new { Post = p, Comment = c }).GroupJoin(_context.Reactions, p => p.Post.Post.Id, r => r.IdPost, (p, r) => new { post = p, Like = r.Where(l => l.Reaction == 1).Count(), Dislike = r.Where(l => l.Reaction == 2).Count() });
                    var book = _context.Books.Where(b => b.idUser == user.Id);
                    var freandFreands = _context.Subscriptions.Where(s => s.IdUser == user.Id).Join(_context.Users, s => s.IdFreand, u => u.Id, (s, u) => new { Sub = s, User = u }).Select(u => u.User);
                    return new { status = "Ok", message = freandsPost, user = _sessionLogin.user.Id, freand = user, book = book, freandFreands = freandFreands };
                }
                else
                {
                    return new { Status = "Error", message = "IdUser dont find" };
                }
            }
        }
        #endregion

        #region Put
        //Add/Delete/Change reaction
        [HttpPut]
        public object Reaction([FromForm] Reactions reactions)
        {
            //var qerty = _context.Reactions.Where(r => r.IdPost == reactions.IdPost).Where(r => r.IdUser == _sessionLogin.user.Id).Join(_context.Posts, r => r.IdPost, p => p.Id, (r, p) => new { Ract = r, Post = p }).FirstOrDefault();
            var qerty = _context.Reactions.Where(r => r.IdPost == reactions.IdPost).Where(r => r.IdUser == _sessionLogin.user.Id).FirstOrDefault();
            if (qerty != null)
            {
                if (qerty.Reaction == reactions.Reaction && qerty.Reaction == 1)
                {
                    qerty.Reaction = 0;
                    _context.Reactions.Update(qerty);
                }
                else if (qerty.Reaction == reactions.Reaction && qerty.Reaction == 2)
                {
                    qerty.Reaction = 0;
                    _context.Reactions.Update(qerty);
                }
                else if (qerty.Reaction != reactions.Reaction && reactions.Reaction == 1)
                {
                    qerty.Reaction = 1;
                    _context.Reactions.Update(qerty);
                }
                else if (qerty.Reaction != reactions.Reaction && reactions.Reaction == 2)
                {
                    qerty.Reaction = 2;
                    _context.Reactions.Update(qerty);
                }
            }
            else
            {
                _context.Reactions.Add(new Reactions { Id = Guid.NewGuid(), IdPost = reactions.IdPost, IdUser = _sessionLogin.user.Id, Reaction = reactions.Reaction });
            }
            _context.SaveChanges();
            return new { status = "Ok", reactLike = _context.Posts.Where(p => p.Id == reactions.IdPost).Join(_context.Reactions, p => p.Id, r => r.IdPost, (p, r) => new {Post = p, React = r}).Select(r => r.React.Reaction).Where(r => r == 1).Count(), 
                                        reactDislike = _context.Posts.Where(p => p.Id == reactions.IdPost).Join(_context.Reactions, p => p.Id, r => r.IdPost, (p, r) => new { Post = p, React = r }).Select(r => r.React.Reaction).Where(r => r == 2).Count() };
        }
        //Update Post
        [HttpPut("{Id}")]
        public object Update(string Id, [FromBody] string text)
        {
            if (string.IsNullOrEmpty(Id))
            {
                return new { status = "Error", message = "Post dont found" };
            }
            Guid id = new Guid();
            try
            {
                id = Guid.Parse(Id);
            }
            catch
            {
                HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
                return new { Status = "Error", message = "Invalid id format (GUID required)" };
            }
            var post = _context.Posts.Find(id);
            if (post == null)
            {
                return new { status = "Error", message = "Post dont found" };
            }
            post.Text = text;
            _context.SaveChanges();
            return new { status = "Ok" };
        }
        #endregion

        #region Post
        //Vallidation and add Post current user
        [HttpPost]
        public object Post([FromForm] Models.PostUserModel postUser)
        {
            if (_sessionLogin.user != null)
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
                    posts.Delete = Guid.Empty;
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
        #endregion

        #region Delete
        //Delete Post
        [HttpDelete("{id}")]
        public object Delete(string id)
        {
            if(!String.IsNullOrEmpty(id))
            {
                Guid Id = new Guid();
                try
                {
                    Id = Guid.Parse(id);
                }
                catch
                {
                    HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
                    return new { Status = "Error", message = "Invalid id format (GUID required)" };
                }
                var post = _context.Posts.Find(Id);
                if(post != null && post.IdUser == _sessionLogin.user.Id)
                {
                    var delete = new DeleteList();
                    delete.Id = Guid.NewGuid();
                    delete.idUser = _sessionLogin.user.Id;
                    delete.Date = DateTime.Now;
                    delete.idElement = post.Id;
                    //Add id on deletelist
                    post.Delete = delete.Id;
                    _context.Posts.Update(post);
                    //Add delete post into DeleteList
                    _context.DeleteList.Add(delete);
                    _context.SaveChanges();
                    return new { status = "Ok", message = "Ok" };
                }
                return new { status = "Error", message = "Post dont found" };
            }
            return new { status = "Error", message = "Id is empty" };
        }
        #endregion
    }
}
