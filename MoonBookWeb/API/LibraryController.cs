using Microsoft.AspNetCore.Mvc;

namespace MoonBookWeb.API
{
    [Route("api/library")]
    [ApiController]
    public class LibraryController : ControllerBase
    {
        private readonly AddDbContext _context;

        public LibraryController(AddDbContext context)
        {
            _context = context;
        }
        [HttpGet("{Search}")]
        public object Search(string Search)
        {
            if(String.IsNullOrEmpty(Search))
            {
                return new { status = "Ok", message = "Search is empty" };
            }
            Search = Search.ToLower().Replace(" ", "");
            var books = _context.Books.Where(b => b.Title!.ToLower().Replace(" ", "") == Search);
            if(books.Count() == 0)
            {
                books = _context.Books.Where(b => b.Author!.ToLower().Replace(" ", "") == Search);
            }
            return new { status = "Ok", message = books };
        }
        //Get All books
        [HttpPost]
        public object Put([FromForm]Models.FilterBookModel filterBook)
        {
            var books = _context.Books.Where(b => b.Delete == Guid.Empty).ToList().GroupJoin(_context.BookRatings, b => b.Id, br => br.IdBook, (b, br) => new { Book = b, Rating = br });
            if (!String.IsNullOrEmpty(filterBook.Genry)) 
            {
                books = books.Where(b => b.Book.Genry == filterBook.Genry);
            }
            if(filterBook.Date)
            {
                books = books.OrderByDescending(b => b.Book.Date);
            }
            if(filterBook.Alphabet)
            {
                books = books.OrderByDescending(b => b.Book.Title);
            }
            if(filterBook.Rating)
            {
                books = books.OrderByDescending(b => b.Rating.Sum(r => r.Grade) / b.Rating.Count());
            }
            return new { status = "Ok", message = books };
        }
    }
}
