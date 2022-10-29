namespace MoonBookWeb.DAL.Entities
{
    public class CommentsBook
    {
        public Guid? Id { get; set; }
        public string? Text { get; set; }
        public DateTime? Date { get; set; }
        public Guid? Answer { get; set; }
        public Guid? IdUser { get; set; }
        public Guid? IdBook { get; set; }
        public Guid? Delete { get; set; }
    }
}
