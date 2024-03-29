﻿using Microsoft.AspNetCore.Mvc;
using MoonBookWeb.Services;

namespace MoonBookWeb.Controllers
{
    public class LoginController : Controller
    {
        private readonly AddDbContext _context;
        private readonly Hesher _hesher;
        private readonly ISessionLogin _sessionLogin;
        private readonly ChekUser _chekUser;
        String[] err;
        public LoginController(ChekUser chekUser, AddDbContext context, Hesher hesher, ISessionLogin sessionLogin)
        {
            _chekUser = chekUser;
            _context = context;
            _hesher = hesher;
            _sessionLogin = sessionLogin;
        }

        public IActionResult Index()
        {
            String err = HttpContext.Session.GetString("RegError")!;
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
        public async Task<RedirectResult> Login(String Login, String Password)
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
                    user.Online = true;
                    await _context.SaveChangesAsync();
                    //Add user session and include middelwere
                    HttpContext.Session.SetString("UserId", user.Id.ToString());
                    return Redirect("/");
                }
                else
                {
                    err[12] = "Password wrong";
                }
            }
            //Add Error Session
            HttpContext.Session.SetString("RegError", String.Join(";", err));
            return Redirect("/Login/Index");
        }
        //Validation and add database
        [HttpPost]
        public async Task<IActionResult> RegUser(Models.RegUserModel userModel)
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
                    //create avatar neme, path and save in folder
                    AvatarName = Guid.NewGuid().ToString() + Path.GetExtension(userModel.Avatar.FileName);
                    await userModel.Avatar.CopyToAsync(new FileStream("./wwwroot/img/" + AvatarName, FileMode.Create));
                }
                var user = new User();
                user.PassSalt = _hesher.Hesh(DateTime.Now.ToString());                  //Create and hesh PassSalt
                user.Password = _hesher.Hesh(userModel?.Password + user.PassSalt);      //Hesh password and pastsalt
                user.PhotoName = AvatarName;                                            //Save in database only avatar
                user.Email = userModel?.Email;
                user.Name = userModel?.Name;
                user.Surname = userModel?.Surname;
                user.Login = userModel?.Login;
                user.RegMoment = DateTime.Now;
                //Add User
                await _context.Users.AddAsync(user);
                await _context.SaveChangesAsync();
                return Redirect("/Login/Index");
            }
            
            HttpContext.Session.SetString("RegError", String.Join(";", err));
            return Redirect("/Login/Registration");
        }
        //Edit data user
        public async Task<IActionResult> EditUser(Models.RegUserModel userModel)
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
                    user!.Name = userModel?.Name;
                }
                if(user?.Surname != userModel?.Surname)
                {
                    user!.Surname = userModel?.Surname;
                }
                if (user?.Status != userModel?.Status)
                {
                    user!.Status = userModel?.Status;
                }
                if (user?.Email != userModel?.Email)
                {
                    user!.Email = userModel?.Email;
                }
                if(user?.Login != userModel?.Login)
                {
                    user!.Login = userModel?.Login;
                }
                if (userModel?.Avatar != null)
                {
                    //Change Avatar
                    String AvatarName = Guid.NewGuid().ToString() + Path.GetExtension(userModel.Avatar.FileName);
                    await userModel.Avatar.CopyToAsync(new FileStream("./wwwroot/img/" + AvatarName, FileMode.Create));
                    System.IO.File.Delete("./wwwroot/img/" + user?.PhotoName);
                    user!.PhotoName = AvatarName;
                }
                if(!String.IsNullOrEmpty(userModel?.Password) && userModel?.Password != user?.Password)
                {
                    if(userModel?.ConfirmPass == userModel?.Password)
                    {
                        user!.PassSalt = _hesher.Hesh(DateTime.Now.ToString());
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
                _context.Users.Update(user!);
                await _context.SaveChangesAsync();
                return Redirect("/");
            }
            HttpContext.Session.SetString("RegError", String.Join(";", err));
            return Redirect("/Login/Update");
        }
        //Exit and change online
        public async Task<IActionResult> Exit()
        {
            var user = await _context.Users.FindAsync(_sessionLogin.user.Id);
            user!.Online = false;
            await _context.SaveChangesAsync();
            HttpContext.Session.Remove("UserId");
            return Redirect("/Login/Index");
        }
        //Update User
        public IActionResult Update()
        {
            String err = HttpContext.Session.GetString("RegError")!;
            if (err != null)
            {
                ViewData["Error"] = err;
                ViewData["err"] = err.Split(';');
                HttpContext.Session.Remove("RegError");
            }
            if(_sessionLogin.user != null)
            {
                ViewData["AuthUser"] = _sessionLogin?.user;
                return View();
            }
            return Redirect("/Login/Index");
        }
        //Fegistration page
        public IActionResult Registration()
        {
            String err = HttpContext.Session.GetString("RegError")!;
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
