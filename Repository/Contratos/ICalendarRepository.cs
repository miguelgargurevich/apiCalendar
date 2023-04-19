using apiCalendar.Entidades;
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
        Task<IEnumerable<EventTypeBE>> GetEventTypesAsync();
        Task<CalendarBE> PostEventAddAsync(CalendarBE calendarBE);
        Task<CalendarBE> PostEventUpdAsync(CalendarBE calendarBE);
        Task<CalendarBE> PostEventDelAsync(CalendarBE calendarBE);
        Task<IEnumerable<CalendarBE>> GetCalendarAsync(int id); 

        #endregion

    }

}
