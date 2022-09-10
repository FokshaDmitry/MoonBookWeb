using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MoonBookWeb.Services;

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
        [HttpGet]
        public object Get()
        {
            var books =  _context.Books.Where(b => b.Delete == Guid.Empty).OrderByDescending(b => b.Date);
            return new { status = "Ok", message = books };
        }
    }
}
