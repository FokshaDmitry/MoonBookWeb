using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibProtocol
{
    [Serializable]
    public class Online
    {
        public List<LibProtocol.Models.Posts> posts { get; set; }
        public List<LibProtocol.Models.User> users { get; set; }
        public List<LibProtocol.Models.Comments> comments { get; set; }
        public List<LibProtocol.Models.Subscriptions> subscriptions { get; set; }
    }
}
