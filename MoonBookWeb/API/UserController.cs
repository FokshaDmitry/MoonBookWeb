using Microsoft.AspNetCore.Mvc;
using MoonBookWeb.Services;

namespace MoonBookWeb.API
{
    [Route("api/user")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly ISessionLogin _sessionLogin;
        private readonly AddDbContext _context;

        public UserController(ISessionLogin sessionLogin, AddDbContext context)
        {
            _sessionLogin = sessionLogin;
            _context = context;
        }
        //find freands current user
        [HttpGet]
        public object OnlineFreands()
        {
            var freand = _context.Subscriptions.Where(s => s.IdUser == _sessionLogin.user.Id).Join(_context.Users, s => s.IdFreand, u => u.Id, (s, u) => new { Sub = s, User = u }).Select(u => u.User).Where(u => u.Online == true);
            return new { status = "Ok", message = freand };
        }
        //Books current user
        [HttpGet("{Book}")]
        public object Boooks()
        {
            var books = _context.Books.Where(b => b.idUser == _sessionLogin.user.Id);
            return new { status = "Ok", message = books };
        }
        //delete comment current user
        [HttpDelete("{id}")]
        public object Delete(string id)
        {
            if (!String.IsNullOrEmpty(id))
            {
                var Id = Guid.Parse(id);
                var comment = _context.Comments.Find(Id);
                if (comment != null)
                {
                    var delete = new DeleteList();
                    delete.Id = Guid.NewGuid();
                    delete.idUser = _sessionLogin.user.Id;
                    delete.Date = DateTime.Now;
                    delete.idElement = comment.Id;
                    //Add id on deletelist
                    comment.Delete = delete.Id;
                    _context.Comments.Update(comment);
                    //Add delete comment into DeleteList
                    _context.DeleteList.Add(delete);
                    _context.SaveChanges();
                    return new { status = "Ok", message = "Ok" };
                }
                return new { status = "Error", message = "Post dont found" };
            }
            return new { status = "Error", message = "Id is empty" };
        }
    }
}
