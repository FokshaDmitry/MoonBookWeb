using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibProtocol.Models
{
    [Serializable]
    public class Comments
    {
        public Guid Id { get; set; }
        public string? Text { get; set; }
        public DateTime Date { get; set; }
        public Guid idUser { get; set; }
        public Guid idPost { get; set; }
    }
}
