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
        public string? CoverName { get; set; }
        public string? Author { get; set; }
        public String? TextContent { get; set; }
        public DateTime? Date { get; set; }
        public String? Genry { get; set; }
        public Guid? idUser { get; set; }
    }
}
