using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace apiCalendar.Entidades
{
    public class CreditLineBE : BaseBE
    {
        public string foundingType { get; set; }
        public double cashBalance { get; set; }
        public double monthlyRevenue { get; set; }
        public double requestedapiCalendar { get; set; }
        public string requestedDate { get; set; }
        public double? recommendedapiCalendar { get; set; }
        public string requestAccepted { get; set; }
        public string requestMessage { get; set; }
        public int? requestNumbers { get; set; }


    }
}
