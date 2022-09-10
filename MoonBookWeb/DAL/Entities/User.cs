using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace MoonBookWeb
{
    [Serializable]
    public class User
    {
        public Guid Id { get; set; }
        public String? Name { get; set; }
        public String? Surname { get; set; }
        public String? Email { get; set; }
        public String? Status { get; set; }
        public DateTime DateOfBith { get; set; }
        public String? Login { get; set; }
        [JsonIgnore]
        public String? Password { get; set; }
        [JsonIgnore]
        public String? PassSalt { get; set; }
        public String? PhotoName { get; set; }
        public DateTime RegMoment { get; set; }
        public bool Online { get; set; }
        //key for delete list
        public Guid Delete { get; set; }
        [NotMapped]
        public byte[]? Phpto { get; set; }
    }
}
