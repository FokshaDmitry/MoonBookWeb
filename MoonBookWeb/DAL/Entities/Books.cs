using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoonBookWeb
{
    [Serializable]
    public class Books
    {
        public Guid Id { get; set; }
        public string? Title { get; set; }
        public byte[]? CoverImage { get; set; }
        public string? Author { get; set; }
        public String? TextContent { get; set; }
        public Guid? idUser { get; set; }
    }
}
