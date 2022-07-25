using Microsoft.AspNetCore.Mvc;
using MoonBookWeb.Services;

namespace MoonBookWeb.Controllers
{
    public class LoginController : Controller
    {
        private readonly AddDbContext _context;
        private readonly Services.Hesher _hesher;
        private readonly ISessionLogin _sessionLogin;
        private readonly ChekUser _chekUser;
        String[] err;
        public LoginController(ChekUser chekUser, AddDbContext context, Services.Hesher hesher, ISessionLogin sessionLogin)
        {
            _chekUser = chekUser;
            _context = context;
            _hesher = hesher;
            _sessionLogin = sessionLogin;
        }

        public IActionResult Index()
        {
            String err = HttpContext.Session.GetString("RegError");
            if (!String.IsNullOrEmpty(err))
            {
                ViewData["ErrorLog"] = err;
                ViewData["errlog"] = err.Split(';');
                HttpContext.Session.Remove("RegError");
            }
            else 
            {
                ViewData["AuthUser"] = _sessionLogin?.user;
            }
            return View();
        }
        //Check login, password and find user
        [HttpPost]
        public RedirectResult Login(String Login, String Password)
        {
            err = new String[13];
            if(String.IsNullOrEmpty(Login))
            {
                err[9] = "Enter Login";
            }
            if(String.IsNullOrEmpty(Password))
            {
                err[10] = "Enter Password";
            }
            var user = _context.Users.Where(u => u.Login == Login).FirstOrDefault();
            if (user == null)
            {
                err[11] = "User don't find";

            }
            else
            {
                if (user.Password == _hesher.Hesh(Password + user.PassSalt))
                {
                    HttpContext.Session.SetString("UserId", user.Id.ToString());
                    return Redirect("/");
                }
                else
                {
                    err[12] = "Password wrong";
                }
            }
            HttpContext.Session.SetString("RegError", String.Join(";", err));
            return Redirect("/Login/Index");
        }
        //Validation and add database
        [HttpPost]
        public IActionResult RegUser(Models.RegUserModel userModel)
        {
            err = _chekUser.Chek(userModel);
            bool isValid = true;
            foreach (string error in err)
            {
                if (!String.IsNullOrEmpty(error)) isValid = false;
            }
            if(isValid)
            {
                String AvatarName = "";
                if (userModel?.Avatar != null)
                {
                    AvatarName = Guid.NewGuid().ToString() + Path.GetExtension(userModel.Avatar.FileName);
                    userModel.Avatar.CopyToAsync(
                        new FileStream(
                            "./wwwroot/img/" + AvatarName,
                            FileMode.Create));
                }
                var user = new User();
                user.PassSalt = _hesher.Hesh(DateTime.Now.ToString());
                user.Password = _hesher.Hesh(userModel?.Password + user.PassSalt);
                user.PhotoName = AvatarName;
                user.Email = userModel?.Email;
                user.Name = userModel?.Name;
                user.Surname = userModel?.Surname;
                user.Login = userModel?.Login;
                user.RegMoment = DateTime.Now;
                _context.Users.Add(user);
                _context.SaveChanges();
                return Redirect("/Login/Index");
            }
            
            HttpContext.Session.SetString("RegError", String.Join(";", err));
            return Redirect("/Login/Registration");
        }
        //Edit data user
        public IActionResult EditUser(Models.RegUserModel userModel)
        {
            var user = _sessionLogin?.user;
            if(user == null)
            {
                return Redirect("/Login/Index");
            }
            err = _chekUser.Chek(userModel);
            bool isValid = true;
            foreach (string error in err)
            {
                if (error == "Enter Password" || error == "Password don't confirm") break;
                if (!String.IsNullOrEmpty(error)) isValid = false;
            }
            
            if (isValid)
            {
                if (user?.Name != userModel.Name)
                {
                    user.Name = userModel?.Name;
                }
                if(user?.Surname != userModel?.Surname)
                {
                    user.Surname = userModel?.Surname;
                }
                if(user?.Email != userModel?.Email)
                {
                    user.Email = userModel?.Email;
                }
                if(user?.Login != userModel?.Login)
                {
                    user.Login = userModel?.Login;
                }
                if (userModel?.Avatar != null)
                {
                    String AvatarName = Guid.NewGuid().ToString() + Path.GetExtension(userModel.Avatar.FileName);
                    userModel.Avatar.CopyToAsync(
                        new FileStream(
                            "./wwwroot/img/" + AvatarName,
                            FileMode.Create));
                    System.IO.File.Delete("./wwwroot/img/" + user?.PhotoName);
                    user.PhotoName = AvatarName;
                }
                if(!String.IsNullOrEmpty(userModel?.Password) && userModel?.Password != user?.Password)
                {
                    if(userModel?.ConfirmPass == userModel?.Password)
                    {
                        user.PassSalt = _hesher.Hesh(DateTime.Now.ToString());
                        user.Password = _hesher.Hesh(userModel?.Password + user.PassSalt);
                    }
                    else
                    {
                        err[7] = "Password don't confirm";
                    }
                }
                else if(userModel?.Password == user?.Password)
                {
                    err[6] = "Create new password";
                }
                _context.Users.Update(user);
                _context.SaveChanges();
                return Redirect("/Home/Index");
            }
            HttpContext.Session.SetString("RegError", String.Join(";", err));
            return Redirect("/Login/Update");
        }
        public IActionResult Exit()
        {
            HttpContext.Session.Remove("UserId");
            return Redirect("/Login/Index");
        }
        public IActionResult Update()
        {
            ViewData["user"] = _sessionLogin?.user;
            String err = HttpContext.Session.GetString("RegError");
            if (err != null)
            {
                ViewData["Error"] = err;
                ViewData["err"] = err.Split(';');
                HttpContext.Session.Remove("RegError");
            }
            return View();
        }
        public IActionResult Registration()
        {
            String err = HttpContext.Session.GetString("RegError");
            if (err != null)
            {
                ViewData["Error"] = err;
                ViewData["err"] = err.Split(';');
                HttpContext.Session.Remove("RegError");
            }
            return View();
        }
    }
}
