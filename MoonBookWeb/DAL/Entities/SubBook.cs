using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoonBookWeb
{
    [Serializable]
    public class SubBook
    {
        public Guid? Id { get; set; }
        public Guid? idBook { get; set; }
        public Guid? idUser { get; set; }
    }
}
