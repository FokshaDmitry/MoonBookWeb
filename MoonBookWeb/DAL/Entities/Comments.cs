﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoonBookWeb
{
    [Serializable]
    public class Comments
    {
        public Guid Id { get; set; }
        public string? Text { get; set; }
        public string? Quote { get; set; }
        public string? Link { get; set; }
        public Guid Answer { get; set; }
        public DateTime Date { get; set; }
        public Guid idUser { get; set; }
        public Guid idPost { get; set; }
        public Guid Delete { get; set; }

    }
}
