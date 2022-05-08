using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibProtocol
{
    [Serializable]
    public class Response
    {
        public bool succces { get; set; }
        public string? StatusTxt { get; set; }
        public ResponseCode code { get; set; }
        public object? data { get; set; }
    }
}
