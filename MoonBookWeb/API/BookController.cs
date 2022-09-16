using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MoonBookWeb.Services;

namespace MoonBookWeb.API
{
    [Route("api/[controller]")]
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

        [HttpGet("{Book}")]
        public object UserBook()
        {
            var books = _context.Books.Where(b => b.idUser == _sessionLogin.user.Id);
            if (books == null)
            {
                return new { status = "Error", message = "Books don't find" };
            }
            return new { status = "Ok", message = books };
        }
        //[HttpGet("{Library}")]
        //public object UserLibrary()
        //{
        //    return new { };
        //}
    }
}
