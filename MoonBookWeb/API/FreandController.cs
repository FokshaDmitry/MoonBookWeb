using Microsoft.AspNetCore.Mvc;
using MoonBookWeb.Services;
using System.Xml.Linq;

namespace MoonBookWeb.API
{
    [Route("api/freand")]
    [ApiController]
    public class FreandController : ControllerBase
    {
        private readonly AddDbContext _context;
        private readonly ISessionLogin _sessionLogin;

        public FreandController(AddDbContext context, ISessionLogin sessionLogin)
        {
            _sessionLogin = sessionLogin;
            _context = context;
        }
        #region Get
        //Get current user Freands
        [HttpGet]
        public object Get()
        {
            var sub = _context.Subscriptions.Where(s => s.IdUser == _sessionLogin.user.Id);
            var freands = _context.Subscriptions.Where(s => s.IdUser == _sessionLogin.user.Id).ToList().Join(_context.Users, s => s.IdFreand, u => u.Id, (s, u) => new { Sub = s, User = u }).GroupJoin(sub, u => u.User.Id, s => s.IdFreand, (u, s) => new { User = u, Sub = s }).Select(u => u.User); 
            if (freands.Any())
            {
                return new {status = "Ok", message = freands };
            }
            return new {status = "Ok", message = "You don't have freands" };
        }
        #endregion

        #region Put
        //Search freand of full name
        [HttpPut("{Name}")]
        public object Search(string Name)
        {
            if (!String.IsNullOrEmpty(Name))
            {
                Name = Name.Replace(" ", "").ToLower();
                var sub = _context.Subscriptions.Where(s => s.IdUser == _sessionLogin.user.Id);
                var users = _context.Users.Where(s => s.Name.ToLower() + s.Surname.ToLower() == Name).ToList().GroupJoin(sub, u => u.Id, s => s.IdFreand, (u, s) => new { User = u, Sub = s});
                if (users.Select(u => u.User).Count() > 0)
                {
                    return new { status = "Ok", message = users};
                }
                else
                {
                    return new { status = "Undefinded", message = "User dont find" };
                }
            }
            return new { status = "Error", message = "No data to request" };
        }
        #endregion

        #region Post
        //Add or delete freand
        [HttpPost("{id}")]
        public object Follow(string id)
        {
            bool b = true;
            if (String.IsNullOrEmpty(id))
            {
                return new { status = "Error", message = "User is empty" };
            }
            Guid Id = new Guid();
            try
            {
                Id = Guid.Parse(id);
            }
            catch
            {
                HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
                return new { Status = "Error", message = "Invalid id format (GUID required)" };
            }
            var user = _context.Subscriptions.Where(s => s.IdUser == _sessionLogin.user.Id).Where(s => s.IdFreand == Id).FirstOrDefault();
            if (user == null)
            {
                //Add new sud current user
                _context.Subscriptions.Add(new Subscriptions { Id = Guid.NewGuid(), IdFreand = Id, IdUser = _sessionLogin.user.Id });
                b = true;
            }
            //Delete freand
            else
            {
                _context.Subscriptions.Remove(user);
                b = false;
            }
            _context.SaveChanges();
            return new { status = "Ok", message = b };
        }
        #endregion
    }
}
