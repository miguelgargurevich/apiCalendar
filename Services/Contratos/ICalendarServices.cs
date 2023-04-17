﻿using apiCalendar.Entidades;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace apiCalendar.Services.Contratos
{
    public interface ICalendarServices
    {

        #region "Metodos o procedimientos"
        RespuestaBE ProcesarLineaCredito(CreditLineBE creditLineBE);

        IEnumerable<EventTypeBE> GetEventTypes();

        Task<IEnumerable<EventTypeBE>> GetEventTypesAsync();
        Task<CalendarBE> PostEventAddAsync(CalendarBE calendarBE);
        Task<CalendarBE> PostEventUpdAsync(CalendarBE calendarBE);
        Task<CalendarBE> PostEventDelAsync(CalendarBE calendarBE);
        Task<IEnumerable<CalendarBE>> GetCalendarAsync(int id);
        

        #endregion

    }

}
