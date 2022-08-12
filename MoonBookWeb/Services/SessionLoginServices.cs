namespace MoonBookWeb.Services
{
    public class SessionLoginServices : ISessionLogin
    {
        private readonly AddDbContext _context;
        public User user { get; set; }
        public List<Guid> userFreands { get; set; }

        public SessionLoginServices(AddDbContext context)
        {
            _context = context;
        }

        public void Set(string id)
        {
            user = _context.Users.Find(Guid.Parse(id));
            userFreands = _context.Users.Select(u => u.Id).Where(u => u == user.Id).ToList();
        }
    }
}
