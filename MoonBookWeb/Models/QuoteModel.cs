namespace MoonBookWeb.Models
{
    public class QuoteModel
    {
        public Guid Id { get; set; }
        public string? Quote { get; set; }
        public int Search { get; set; }
        public string? Text { get; set; }
    }
}
