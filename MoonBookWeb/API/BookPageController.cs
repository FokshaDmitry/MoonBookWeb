using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
        //Get book for Id
        [HttpGet("{Id}")]
        public async Task<object> GetBook(string Id)
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
                HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
                return new { status = "Error", message = "Book don't found" };
            }
            var grades = _context.BookRatings.Where(r => r.IdBook == id).Count();
            var sub = _context.SubBooks.Where(r => r.idBook == id).Count();
            var rating = _context.BookRatings.Where(br => br.IdBook == id).AsNoTracking();
            return new { status = "Ok", message = book, follow = follow, grades = grades, sub = sub, rating = rating }; ;
        }
        //Add Grade for book
        [HttpPost("{Id}")]
        public async Task<object> PostGrade(string Id, [FromForm]int Grade)
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
            var grade = _context.BookRatings.Where(g => g.IdUser == _sessionLogin.user.Id).Where(g => g.IdBook == id).AsNoTracking().FirstOrDefault();
            if(grade == null)
            {
                BookRating bookRating = new BookRating();
                bookRating.Id = Guid.NewGuid();
                bookRating.Grade = Grade;
                bookRating.IdUser = _sessionLogin.user.Id;
                bookRating.IdBook = id;
                await _context.BookRatings.AddAsync(bookRating);
                await _context.SaveChangesAsync();
            }
            else
            {
                grade.Grade = Grade;
                _context.BookRatings.Update(grade);
                await _context.SaveChangesAsync();
            }
            var grades = _context.BookRatings.Where(r => r.IdBook == id).AsNoTracking();
            return new { status = "Ok", message = grades };
        }
    }
}
