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
            //Validation User
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
        public string[] ChekAddBook(Models.AddBookModel book)
        {
            //Vallidation Book
            var err = new String[6];
            if (book == null)
            {
                err[0] = "Error querty";
            }
            if (String.IsNullOrEmpty(book.Author))
            {
                err[1] = "Write Author";
            }
            if (String.IsNullOrEmpty(book.Title))
            {
                err[2] = "Write Title";
            }
            if (book.Genry == "Choose Genry")
            {
                err[3] = "Choose Genry";
            }
            if (String.IsNullOrEmpty(book.Annotation))
            {
                err[4] = "Write Anotation";
            }
            if (String.IsNullOrEmpty(book.TextContent))
            {
                err[5] = "Write Anotation";
            }
            return err;
        }
    }
}
