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
        //Get All books
        [HttpPost]
        public object Put([FromForm]Models.FilterBookModel filterBook)
        {
            var books = _context.Books.Where(b => b.Delete == Guid.Empty);
            if (!String.IsNullOrEmpty(filterBook.Genry)) 
            {
                books = books.Where(b => b.Genry == filterBook.Genry);
            }
            if(filterBook.Date)
            {
                books = books.OrderByDescending(b => b.Date);
            }
            if(filterBook.Alphabet)
            {
                books = books.OrderByDescending(b => b.Title);
            }
            return new { status = "Ok", message = books };
        }
    }
}
