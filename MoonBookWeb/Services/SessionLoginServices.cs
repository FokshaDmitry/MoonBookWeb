namespace MoonBookWeb.Services
{
    public class SessionLoginServices : ISessionLogin
    {
        private readonly AddDbContext _context;
        public User user { get; set; }

        public SessionLoginServices(AddDbContext context)
        {
            _context = context;
        }

        public void Set(string id)
        {
            user = _context.Users.Find(Guid.Parse(id));
        }
    }
}
