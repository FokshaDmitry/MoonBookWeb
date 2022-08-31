using EpubSharp;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MoonBookWeb.Models;
using MoonBookWeb.Services;

namespace MoonBookWeb.Controllers
{
    public class BooksController : Controller
    {
        private readonly ISessionLogin _sessionLogin;
        private readonly ChekUser _chekUser;
        private readonly AddDbContext _context;
        public BooksController(ISessionLogin sessionLogin, ChekUser chekUser, AddDbContext context)
        {
            _sessionLogin = sessionLogin;
            _chekUser = chekUser;
            _context = context;
        }

        public IActionResult Index()
        {
            if (_sessionLogin.user != null)
            {
                ViewData["AuthUser"] = _sessionLogin?.user;
                return View();
            }
            return Redirect("/Login/Index");
        }
        [HttpPut]
        public IActionResult eBook(IFormFile efaile)
        {
            EpubBook epubBook = new EpubBook();
            epubBook = EpubReader.Read(efaile.OpenReadStream().ReadToEnd());
            
            Models.AddBookModel model = new Models.AddBookModel();
            model.Author = epubBook.Authors.FirstOrDefault();
            model.Title = epubBook.Title;
            model.TextContent = epubBook.ToPlainText();
            return Json(new {status = "Ok", info = model, cover = epubBook.CoverImage, file = efaile});
        }
        [HttpPost]
        public IActionResult AddBook([FromForm]AddBookModel book)
        {
            
            var err = _chekUser.ChekAddBook(book);
            bool isValid = true;
            foreach (string error in err)
            {
                if (!String.IsNullOrEmpty(error)) isValid = false;
            }
            ViewData["ErrorBook"] = err;
            if(isValid)
            {
                String CoverName = "";
                if (book?.CoverImage != null)
                {
                    CoverName = Guid.NewGuid().ToString() + Path.GetExtension(book.CoverImage.FileName);
                    book.CoverImage.CopyToAsync(
                        new FileStream(
                            "./wwwroot/cover_img" + CoverName,
                            FileMode.Create));
                }
                var books = new Books();
                books.Id = Guid.NewGuid();
                books.Title = book?.Title;
                books.Author = books.Author;
                books.CoverName = CoverName;
                books.idUser = _sessionLogin.user.Id;
                books.TextContent = $"{book?.Genry}\n {book?.Author}\n {book?.Title}\n {book?.Annotation}\n\n {book?.TextContent}";
                books.Date = DateTime.Now;
                books.Genry = book?.Genry;
                _context.Books.Add(books);
                _context.SaveChanges();
                return Redirect("/User/Index");
            }
            return View();
        }
    }
}
