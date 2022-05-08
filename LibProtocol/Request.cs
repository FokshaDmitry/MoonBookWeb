using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibProtocol
{
    [Serializable]
    public class Request
    {
        public Command command { get; set; }
        public object? data { get; set; }
    }
}
