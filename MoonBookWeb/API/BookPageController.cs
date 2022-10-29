using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MoonBookWeb.DAL.Entities;
using MoonBookWeb.Models;
using MoonBookWeb.Services;

namespace MoonBookWeb.API
{
    [Route("api/BookPage")]
    [ApiController]
    public class BookPageController : ControllerBase
    {
        private readonly AddDbContext _context;
        private readonly ISessionLogin _sessionLogin;

        public BookPageController(AddDbContext context, ISessionLogin sessionLogin)
        {
            _context = context;
            _sessionLogin = sessionLogin;
        }

        [HttpGet("{Id}")]
        public object GetBook(string Id)
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
            var book = _context.Books.Find(id);
            var follow = _context.SubBooks.Where(s => s.idUser == _sessionLogin.user.Id).Select(s => s.idBook).Contains(book?.Id);
            if (book == null)
            {
                HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
                return new { status = "Error", message = "Book don't found" };
            }
            return new { status = "Ok", message = book, follow = follow}; ;
        }
        [HttpPost]
        public object PostComment([FromForm] AddComentModel addComent)
        {
            if (addComent == null)
            {
                HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
                return new { status = "Error", message = "Bad Request" };
            }
            var idAnswer = _context.Users.Where(u => u.Login == addComent.Answer).Select(u => u.Id).FirstOrDefault();    
            CommentsBook comments = new CommentsBook(); 
            comments.Id = Guid.NewGuid();
            comments.IdUser = _sessionLogin.user.Id;
            comments.IdBook = addComent.id;
            comments.Date = DateTime.Now;
            comments.Text = addComent.Text;
            comments.Answer = idAnswer == null ? Guid.Empty : idAnswer;
            comments.Delete = Guid.Empty;
            _context.CommentsBooks.Add(comments);
            _context.SaveChanges();
            return new { status = "Ok", message = "Ok" };
        }
    }
}
