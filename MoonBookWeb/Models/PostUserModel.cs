namespace MoonBookWeb.Models
{
    public class PostUserModel
    {
        public string? TitlePost { get; set; }
        public string? TextPost { get; set; }
        public IFormFile? ImagePost { get; set; }
    }
}
