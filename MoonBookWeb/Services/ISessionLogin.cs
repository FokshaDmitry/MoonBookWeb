namespace MoonBookWeb.Services
{
    //Interface User Type
    public interface ISessionLogin
    {
        public User user { get; set; }
        public void Set(string id);
    }
}
