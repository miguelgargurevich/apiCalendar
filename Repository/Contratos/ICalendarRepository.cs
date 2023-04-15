﻿using apiCalendar.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace apiCalendar.Repository.Contratos
{
    public interface ICalendarRepository
    {

        #region "Metodos o procedimientos"

        IEnumerable<EventTypeBE> GetEventTypes();
        RespuestaBE ProcesarLineaCredito(CreditLineBE creditLineBE);

        #endregion

    }

}