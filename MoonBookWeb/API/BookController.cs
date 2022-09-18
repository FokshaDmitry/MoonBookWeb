using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MoonBookWeb.Services;
using static System.Reflection.Metadata.BlobBuilder;

namespace MoonBookWeb.API
{
    [Route("api/book")]
    [ApiController]
    public class BookController : ControllerBase
    {
        private readonly AddDbContext _context;
        private readonly ISessionLogin _sessionLogin;

        public BookController(AddDbContext context, ISessionLogin sessionLogin)
        {
            _context = context;
            _sessionLogin = sessionLogin;
        }

        [HttpGet]
        public object UserBook()
        {
            var books = _context.Books.Where(b => b.idUser == _sessionLogin.user.Id);
            if (books == null)
            {
                return new { status = "Error", message = "Books don't find" };
            }
            return new { status = "Ok", message = books };
        }
        [HttpGet("{Id}")]
        public object ReadBook(string Id)
        {
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
            var book = _context.Books.Find(id);
            var follow = _context.SubBooks.Where(s => s.idUser == _sessionLogin.user.Id).Select(s => s.idBook).Contains(book.Id);
            if(book == null)
            {
                return new { status = "Error", message = "Book don't find" };
            }
            return new { status = "Ok", message = book, follow =  follow};
        }
        //[HttpGet("{Library}")]
        //public object UserLibrary()
        //{
        //    return new { };
        //}
    }
}
