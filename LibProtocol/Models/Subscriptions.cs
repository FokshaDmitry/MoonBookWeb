using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibProtocol.Models
{
    [Serializable]
    public class Subscriptions
    {
        public Guid Id { get; set; }
        public Guid IdUser { get; set; }
        public Guid IdFreand { get; set; }
        
    }
}
