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
        #region Get
        //Find post current User
        [HttpGet]
        public object Get()
        {
            var comment = _context.Comments.Where(c => c.Delete == Guid.Empty).Join(_context.Users, c => c.idUser, u => u.Id, (c, u) => new {Comment = c, User = u});
            var post = _context.Posts.Where(p => p.IdUser == _sessionLogin.user.Id).Where(p => p.Delete == Guid.Empty).ToList().Join(_context.Users, p => p.IdUser, u => u.Id, (p, u) => new { post = p, user = u }).OrderByDescending(p => p.post.Date).GroupJoin(comment, p => p.post.Id, c => c.Comment.idPost, (p, c) => new { Post = p, Comment = c });
            return new { status = "Ok", message = post };
        }
        #endregion

        #region Put
        //Add/Delete/Change reaction
        [HttpPut]
        public object Reaction([FromForm] Reactions reactions)
        {
            var qerty = _context.Reactions.Where(r => r.IdPost == reactions.IdPost).Where(r => r.IdUser == _sessionLogin.user.Id).Join(_context.Posts, r => r.IdPost, p => p.Id, (r, p) => new { Ract = r, Post = p }).FirstOrDefault();
            if (qerty != null)
            {
                if (qerty.Ract.Reaction == reactions.Reaction && qerty.Ract.Reaction == 1)
                {
                    qerty.Post.Like--;
                    _context.Posts.Update(qerty.Post);
                    _context.Reactions.Remove(qerty.Ract);
                }
                else if (qerty.Ract.Reaction == reactions.Reaction && qerty.Ract.Reaction == 2)
                {
                    qerty.Post.Dislike--;
                    _context.Posts.Update(qerty.Post);
                    _context.Reactions.Remove(qerty.Ract);
                }
                else if (qerty.Ract.Reaction != reactions.Reaction && reactions.Reaction == 1)
                {
                    qerty.Ract.Reaction = 1;
                    qerty.Post.Like++;
                    if (qerty.Post.Dislike != 0) qerty.Post.Dislike--;
                    _context.Posts.Update(qerty.Post);
                    _context.Reactions.Update(qerty.Ract);
                }
                else if (qerty.Ract.Reaction != reactions.Reaction && reactions.Reaction == 2)
                {
                    qerty.Ract.Reaction = 2;
                    qerty.Post.Dislike++;
                    if (qerty.Post.Like != 0) qerty.Post.Like--;
                    _context.Posts.Update(qerty.Post);
                    _context.Reactions.Update(qerty.Ract);
                }
            }
            else
            {
                var q = _context.Posts.Where(p => p.Id == reactions.IdPost).FirstOrDefault();
                if (reactions.Reaction == 1) q.Like++;
                else q.Dislike++;

                _context.Posts.Update(q);
                _context.Reactions.Add(new Reactions { Id = Guid.NewGuid(), IdPost = reactions.IdPost, IdUser = _sessionLogin.user.Id, Reaction = reactions.Reaction });
            }
            _context.SaveChanges();
            return new { status = "Ok", reactLike = _context.Posts.Where(p => p.Id == reactions.IdPost).FirstOrDefault().Like, reactDislike = _context.Posts.Where(p => p.Id == reactions.IdPost).FirstOrDefault().Dislike };
        }
        //Add comment
        [HttpPut("{Comment}")]
        public object Comment([FromForm] Comments comment)
        {
            if (comment != null)
            {
                comment.Id = Guid.NewGuid();
                comment.idUser = _sessionLogin.user.Id;
                comment.Date = DateTime.Now;
                comment.Delete = Guid.Empty;
                _context.Comments.Add(comment);
                _context.SaveChanges();
            }
            var responce = _context.Comments.Where(c => c.Id == comment.Id).Join(_context.Users, c => c.idUser, u => u.Id, (c, u) => new { comment = c, user = u }).FirstOrDefault();
            return new { status = "Ok", message = responce };
        }
        #endregion

        #region Post
        //Vallidation and add current user
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
                    posts.Like = 0;
                    posts.Dislike = 0;
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
        [HttpPost("{Id}")]
        public object Update(string Id, [FromBody] string text)
        {
            if(string.IsNullOrEmpty(Id))
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
