using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MoonBookWeb.API
{
    [Route("api/freand")]
    [ApiController]
    public class FreandController : ControllerBase
    {
        private readonly AddDbContext _context;

        public FreandController(AddDbContext context)
        {
            _context = context;
        }

        [HttpPut]
        public object Search([FromForm]string Name)
        {
            if (!String.IsNullOrEmpty(Name))
            {
                Name = Name.Replace(" ", "").ToLower();
                var name = _context.Users.Where(s => s.Name.ToLower() + s.Surname.ToLower() == Name);
                if (name.Any(u => u.Name != ""))
                {
                    List<User> users = new List<User>();
                    foreach (var user in name)
                    {
                        user.Password = "*";
                        user.PassSalt = "*";
                        users.Add(user);
                    }
                    return new { status = "Ok", message = name };
                }
                else
                {
                    return new { status = "Undefinded", message = "User dont find" };
                }
            }
            return new { status = "Error", message = "No data to request" };
        }
    }
}
