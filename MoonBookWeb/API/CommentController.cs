using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MoonBookWeb.Models;
using MoonBookWeb.Services;

namespace MoonBookWeb.API
{
    [Route("api/Comment")]
    [ApiController]
    public class CommentController : ControllerBase
    {
        private readonly ISessionLogin _sessionLogin;
        private readonly AddDbContext _context;

        public CommentController(ISessionLogin sessionLogin, AddDbContext context)
        {
            _sessionLogin = sessionLogin;
            _context = context;
        }
        //Get Comment for post
        [HttpGet("{Id}")]
        public object GetComment(string Id)
        {
            Guid id = new Guid();
            try
            {
                id = Guid.Parse(Id);
            }
            catch
            {
                HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
                return new { status = "Error", message = "Invalid id format (GUID required)" };
            }
            var comment = _context.Comments.Where(cb => cb.Delete == Guid.Empty).Where(cb => cb.idPost == id).Join(_context.Users, cb => cb.idUser, u => u.Id, (cb, u) => new { Comment = cb, User = u }).AsNoTracking().ToList().GroupJoin(_context.Users, c => c.Comment.Answer, u => u.Id, (c, u) => new { Comment = c, UserAnswer = u }).OrderBy(c => c.Comment.Comment.Date); ;
            return new { status = "Ok", message = comment, user = _sessionLogin.user.Id };
        }
        //Add comment for post
        [HttpPost]
        public async Task<object> Comment([FromForm] AddComentModel commentModel)
        {

            if (commentModel != null)
            {
                var AnswerUser = _context.Users.Where(u => u.Login == commentModel.Answer).AsNoTracking().FirstOrDefault();
                Comments comment = new Comments();
                comment.Id = Guid.NewGuid();
                comment.idUser = _sessionLogin.user.Id;
                comment.Date = DateTime.Now;
                comment.Delete = Guid.Empty;
                comment.Text = commentModel.Text;
                comment.idPost = commentModel.id;
                comment.Answer = AnswerUser == null ? Guid.Empty : AnswerUser.Id;
                comment.Link = "";
                comment.Quote = "";
                await _context.Comments.AddAsync(comment);
                await _context.SaveChangesAsync();
                var responce = _context.Comments.Where(c => c.Id == comment.Id).Join(_context.Users, c => c.idUser, u => u.Id, (c, u) => new { comment = c, user = u }).AsNoTracking().FirstOrDefault();
                return new { status = "Ok", message = responce, answer = AnswerUser };
            }
            else
            {
                return new { status = "Error", message = "Bad Request" };
            }
        }
        //Add comment for book
        [HttpPost("book")]
        public async Task<object> Quote([FromForm] QuoteModel quoteModel) //Model for book comment
        {
            if(quoteModel != null)
            {
                if(quoteModel.Id != Guid.Empty)
                {
                    Comments comment = new Comments();
                    comment.Id = Guid.NewGuid();
                    comment.idUser = _sessionLogin.user.Id;
                    comment.Date = DateTime.Now;
                    comment.idPost = quoteModel.Id;
                    comment.Text = quoteModel.Text;
                    try 
                    {
                        comment.Quote = quoteModel?.Quote?.Substring(0, 30) + "...";
                    }
                    catch // if Quote has more then 30 letters
                    {
                        comment.Quote = quoteModel?.Quote;
                    }
                    comment.Link = $"../Books/UserLibrary?{quoteModel?.Id}&search={quoteModel?.Search}";
                    comment.Answer = Guid.Empty;
                    comment.Delete = Guid.Empty;
                    await _context.Comments.AddAsync(comment);
                    await _context.SaveChangesAsync();
                }
                else
                {
                    return new { status = "Error", message = "Id don't found" };
                }
                return new { status = "Ok", message = "Bad Request" };
            }
            else
            {
                return new { status = "Error", message = "Bad Request" };
            }
        }
        //Update Comment
        [HttpPut("{Id}")]
        public async Task<object> UpdateComment(string Id, [FromBody] string text)
        {
            if (string.IsNullOrEmpty(Id))
            {
                return new { status = "Error", message = "Comment dont found" };
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
            var comment = await _context.Comments.FindAsync(id);
            if (comment == null)
            {
                return new { status = "Error", message = "Commrnt dont found" };
            }
            comment.Text = text;
            _context.SaveChanges();
            return new { status = "Ok" };
        }
        //delete comment current user
        [HttpDelete("{id}")]
        public async Task<object> DeleteComment(string id)
        {
            if (!String.IsNullOrEmpty(id))
            {
                var Id = Guid.Parse(id);
                var comment = await _context.Comments.FindAsync(Id);
                if (comment != null)
                {
                    var delete = new DeleteList();
                    delete.Id = Guid.NewGuid();
                    delete.idUser = _sessionLogin.user.Id;
                    delete.Date = DateTime.Now;
                    delete.idElement = comment.Id;
                    //Add id on deletelist
                    comment.Delete = delete.Id;
                    //Add delete comment into DeleteList
                    await _context.DeleteList.AddAsync(delete);
                    await _context.SaveChangesAsync();
                    return new { status = "Ok", message = "Ok" };
                }
                return new { status = "Error", message = "Post dont found" };
            }
            return new { status = "Error", message = "Id is empty" };
        }
    }
}
