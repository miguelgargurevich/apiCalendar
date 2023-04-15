using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace apiCalendar.Entidades
{
    public class ContentBE
    {
        public string token { get; set; }
        public bool isAllowed { get; set; }
        public string subject { get; set; }
    }
}
