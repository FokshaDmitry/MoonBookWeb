using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
            var books = _context.Books.Where(b => b.idUser == _sessionLogin.user.Id).AsNoTracking();
            if (books == null)
            {
                return new { status = "Error", message = "Books don't find" };
            }
            return new { status = "Ok", message = books };
        }
        [HttpGet("{Id}")]
        public async Task<object> ReadBook(string Id)
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
            var book = await _context.Books.FindAsync(id);
            var follow = _context.SubBooks.Where(s => s.idUser == _sessionLogin.user.Id).Select(s => s.idBook).Contains(book?.Id);
            if (book == null)
            {
                return new { status = "Error", message = "Book don't find" };
            }
            return new { status = "Ok", message = book, follow = follow };
        }
        [HttpPost("{Id}")]
        public async Task<object> SubBook(string Id)
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
            if (_context.SubBooks.Where(s => s.idUser == _sessionLogin.user.Id).Select(s => s.idBook).Contains(id))
            {
                var subuser = _context.SubBooks.AsNoTracking().FirstOrDefault(s => s.idBook == id);
                _context.SubBooks.Remove(subuser!);
                await _context.SaveChangesAsync();
                var sub = _context.SubBooks.Where(r => r.idBook == id).AsNoTracking().Count();
                return new { status = "Ok", message = false, sub = sub };
            }
            else
            {
                var book = await _context.Books.FindAsync(id);
                if (book == null)
                {
                    return new { status = "Error", message = "Book don't find" };
                }
                await _context.SubBooks.AddAsync(new SubBook { Id = Guid.NewGuid(), idBook = book.Id, idUser = _sessionLogin.user.Id });
                await _context.SaveChangesAsync();
                var sub = _context.SubBooks.Where(r => r.idBook == id).AsNoTracking().Count();
                return new { status = "Ok", message = true, sub = sub };
            }
        }
        [HttpPut]
        public object UserLibrary()
        {
            var book = _context.SubBooks.Where(s => s.idUser == _sessionLogin.user.Id).Join(_context.Books, s => s.idBook, b => b.Id, (s, b) => new { Sub = s, Book = b}).Select( b => b.Book).AsNoTracking();
            if (book == null)
            {
                return new { status = "Error", message = "Book don't find" };
            }
            return new { status = "Ok", message = book };
        }
    }
}
