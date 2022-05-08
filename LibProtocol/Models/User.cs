using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibProtocol.Models
{
    [Serializable]
    public class User
    {
        public Guid Id { get; set; }
        public String? Name { get; set; }
        public String? Surname { get; set; }
        public DateTime DateOfBith { get; set; }
        public String? Login { get; set; }
        public String? Password { get; set; }
        public byte[]? Phpto { get; set; }
        public bool Online { get; set; }
    }
}
