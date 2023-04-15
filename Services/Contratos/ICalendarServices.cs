using apiCalendar.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace apiCalendar.Services.Contratos
{
    public interface ICalendarServices
    {

        #region "Metodos o procedimientos"
        RespuestaBE ProcesarLineaCredito(CreditLineBE creditLineBE);

        IEnumerable<EventTypeBE> GetEventTypes();
        #endregion

    }

}
