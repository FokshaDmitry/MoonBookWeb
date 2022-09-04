using EpubSharp;
using System.IO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MoonBookWeb.Models;
using MoonBookWeb.Services;
using System.Text.Json;

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
            String err = HttpContext.Session.GetString("AddBookErr");
            if (_sessionLogin.user != null)
            {
                ViewData["AuthUser"] = _sessionLogin?.user;
                ViewData["errlog"] = err?.Split(';');
                HttpContext.Session.Remove("AddBookErr");
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
            HttpContext.Session.SetString("CoverImg", JsonSerializer.Serialize(epubBook.CoverImage));
            return Json(new {status = "Ok", info = model, cover = epubBook.CoverImage });
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
            if(isValid)
            {
                String CoverName = "";
                byte[] covertmp = JsonSerializer.Deserialize<byte[]>(HttpContext.Session.GetString("CoverImg"));
                if (covertmp != null)
                {
                    CoverName = Guid.NewGuid().ToString() + ".png";
                    System.IO.File.WriteAllBytes("./wwwroot/img_post/" + CoverName, covertmp);
                }
                else
                {
                    if (book?.CoverImage != null)
                    {
                        CoverName = Guid.NewGuid().ToString() + Path.GetExtension(book.CoverImage.FileName);
                        book.CoverImage.CopyToAsync(
                            new FileStream(
                                "./wwwroot/img_post/" + CoverName,
                                FileMode.Create));
                    }
                }
                HttpContext.Session.Remove("CoverImg");
                var books = new Books();
                books.Id = Guid.NewGuid();
                books.Title = book?.Title;
                books.Author = book?.Author;
                books.CoverName = CoverName;
                books.idUser = _sessionLogin.user.Id;
                books.TextContent = $"{book?.Genry}\n {book?.Author}\n {book?.Title}\n {book?.Annotation}\n\n {book?.TextContent}";
                books.Date = DateTime.Now;
                books.Genry = book?.Genry;
                _context.Posts.Add(new Posts { Id = Guid.NewGuid(), Date = DateTime.Now, Title = books.Title, Image = CoverName, IdUser = _sessionLogin.user.Id });
                _context.Books.Add(books);
                _context.SaveChanges();
                return Redirect("/User/Index");
            }
            HttpContext.Session.SetString("AddBookErr", String.Join(";", err));
            return Redirect("/Books/Index");
        }
    }
}
