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
            String err = HttpContext.Session.GetString("AddBookErr")!;
            if (_sessionLogin.user != null)
            {
                ViewData["AuthUser"] = _sessionLogin?.user;
                ViewData["ErrorBook"] = err?.Split(';');
                HttpContext.Session.Remove("AddBookErr");
                return View();
            }
            return Redirect("/Login/Index");
        }
        public IActionResult UserLibrary()
        {
            if (_sessionLogin.user != null)
            {
                ViewData["UserLibrary"] = "Ok"; 
                ViewData["AuthUser"] = _sessionLogin?.user;
                return View();
            }
            return Redirect("/Login/Index");
        }
        public IActionResult BookPage()
        {
            if (_sessionLogin.user != null)
            {
                ViewData["AuthUser"] = _sessionLogin?.user;
                return View();
            }
            return Redirect("/Login/Index");

        }
        //Add book with ebub file. Parse file and return info
        [HttpPut]
        public IActionResult eBook(IFormFile efaile)
        {
            EpubBook epubBook = new EpubBook();
            epubBook = EpubReader.Read(efaile.OpenReadStream().ReadToEnd());
            
            Models.AddBookModel model = new Models.AddBookModel();
            model.Author = epubBook.Authors.FirstOrDefault();
            model.Title = epubBook.Title;
            model.TextContent = epubBook.ToPlainText();
            //tmp session for the cover to seve it in database 
            HttpContext.Session.SetString("CoverImg", JsonSerializer.Serialize(epubBook.CoverImage));
            //return book info and cover
            return Json(new {status = "Ok", info = model, cover = epubBook.CoverImage });
        }
        //Add Book Page
        [HttpPost]
        public async Task<IActionResult> AddBook([FromForm]AddBookModel book)
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
                
                byte[] covertmp = JsonSerializer.Deserialize<byte[]>(HttpContext.Session.GetString("CoverImg")!)!;

                if (covertmp != null)
                {
                    //Create cover name
                    CoverName = Guid.NewGuid().ToString() + ".png";
                    System.IO.File.WriteAllBytes("./wwwroot/img_post/" + CoverName, covertmp);
                }
                else
                {
                    if (book?.CoverImage != null)
                    {
                        CoverName = Guid.NewGuid().ToString() + Path.GetExtension(book.CoverImage.FileName);
                        await book.CoverImage.CopyToAsync(new FileStream( "./wwwroot/img_post/" + CoverName, FileMode.Create));
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
                //Create post about add book
                await _context.Posts.AddAsync(new Posts { Id = Guid.NewGuid(), Date = DateTime.Now, Title = books.Title, Image = CoverName, IdUser = _sessionLogin.user.Id });
                await _context.Books.AddAsync(books);
                await _context.SaveChangesAsync();
                return Redirect("/User/Index");
            }
            HttpContext.Session.SetString("AddBookErr", String.Join(";", err));
            return Redirect("/Books/Index");
        }
    }
}
