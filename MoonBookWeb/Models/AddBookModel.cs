namespace MoonBookWeb.Models
{
    public class AddBookModel
    {
        //tmp model Book
        public String? Title { get; set; }
        public IFormFile? CoverImage { get; set; }
        public String? Author { get; set; }
        public String? TextContent { get; set; }
        public String? Genry { get; set; }
        public String? Annotation { get; set; }
    }
}
