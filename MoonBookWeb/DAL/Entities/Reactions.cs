using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoonBookWeb
{
    [Serializable]
    public class Reactions
    {
        public Guid Id { get; set; }
        public int Reaction { get; set; }
        public Guid IdUser { get; set; }
        public Guid IdPost { get; set; }
    }
}
