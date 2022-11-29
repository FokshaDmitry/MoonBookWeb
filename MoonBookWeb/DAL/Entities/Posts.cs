using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoonBookWeb
{
    [Serializable]
    public class Posts
    {
        public Guid Id { get; set; }
        public DateTime Date { get; set; }
        public string? Title { get; set; }
        public string? Text { get; set; }
        public string? Image { get; set; }
        public Guid IdUser { get; set; }
        public Guid Delete { get; set; }


    }
}
