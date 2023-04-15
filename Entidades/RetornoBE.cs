using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace apiCalendar.Entidades
{
    public class RetornoBE
    {

        public int code { get; set; }
        public string message { get; set; }

        public ContentBE content { get; set; }

    }
}
