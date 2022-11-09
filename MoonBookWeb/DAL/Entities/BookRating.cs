namespace MoonBookWeb.DAL.Entities
{
    public class BookRating
    {
        public Guid Id { get; set; }
        public double Grade { get; set; }
        public Guid IdBook { get; set; }
        public Guid IdUser { get; set; }
    }
}
