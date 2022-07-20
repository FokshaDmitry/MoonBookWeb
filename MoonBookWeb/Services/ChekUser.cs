namespace MoonBookWeb.Services
{
    public class ChekUser
    {
        private AddDbContext _context;

        public ChekUser(AddDbContext context)
        {
            _context = context;
        }
        public string[] Chek(Models.RegUserModel user)
        {
            var err = new String[13];
            if (user == null)
            {
                err[0] = "Incorrect call (no data)";
            }
            if (String.IsNullOrEmpty(user?.Name))
            {
                err[1] = "Enter Name";
            }
            if (String.IsNullOrEmpty(user?.Surname))
            {
                err[2] = "Enter Surname";
            }
            if (user?.DateOfBith == null)
            {
                err[3] = "Enter Date of Both";
            }
            if (String.IsNullOrEmpty(user?.Login))
            {
                err[4] = "Enter Login";
            }
            if (String.IsNullOrEmpty(user?.Email))
            {
                err[5] = "Enter Email";
            }
            if (String.IsNullOrEmpty(user?.Password))
            {
                err[6] = "Enter Password";
            }
            if (user?.Password != user?.ConfirmPass)
            {
                err[7] = "Password don't confirm";
            }
            if (_context.Users.Select(u => u.Login).Contains(user?.Login))
            {
                err[8] = "Login already exists";
            }
            return err;
        }
    }
}
