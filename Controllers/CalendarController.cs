using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using apiCalendar.Entidades;
using apiCalendar.Models;
using apiCalendar.Services.Contratos;
using apiCalendar;

namespace CoreApi.Controllers
{
    [ApiController]
    [Route("api/calendar")]
    public class CalendarController : ControllerBase
    {
        private readonly IConfiguration _config;
        ICalendarServices _CalendarServices;
        private readonly ILogger<CalendarController> _logger;
        string _urlApiSeguridad;
        string _rutaApiSeguridad;

        public CalendarController(
            IConfiguration config,
            ICalendarServices apiCalendarServices,
            ILogger<CalendarController> logger)
        {
            _config = config;
            _CalendarServices = apiCalendarServices;
            _logger = logger;

            _urlApiSeguridad = _config["URL_API_SEGURIDAD"];
            _rutaApiSeguridad = _config["RUTA_API_SEGURIDAD"];
        }


        [Route("getEventTypes")]
        [HttpGet]
        public IActionResult getEventTypes()
        {

            var respuestaBE = _CalendarServices.GetEventTypes();
            return Ok(respuestaBE.ToArray());
        }

        [Route("getEventTypesAsync")]
        [HttpGet]
        public async Task<IActionResult> GetEventTypesAsync()
        {

            var respuestaBE = await _CalendarServices.GetEventTypesAsync();
            return Ok(respuestaBE.ToArray());
        }

        
        [Route("postEventAddAsync")]
        [HttpPost]
        public async Task<IActionResult> PostEventAddAsync([FromBody] CalendarBE calendarBE)
        {

            var respuestaBE = await _CalendarServices.PostEventAddAsync(calendarBE);
            return Ok(respuestaBE);
        }

        [Route("postEventUpdAsync")]
        [HttpPost]
        public async Task<IActionResult> PostEventUpdAsync([FromBody] CalendarBE calendarBE)
        {

            var respuestaBE = await _CalendarServices.PostEventUpdAsync(calendarBE);
            return Ok(respuestaBE);
        }

        [Route("postEventDelAsync")]
        [HttpPost]
        public async Task<IActionResult> PostEventDelAsync([FromBody] CalendarBE calendarBE)
        {

            var respuestaBE = await _CalendarServices.PostEventDelAsync(calendarBE);
            return Ok(respuestaBE);
        }

        
        [Route("getCalendarAsync")]
        [HttpGet]
        public async Task<IActionResult> GetCalendarAsync(int id)
        {

            var respuestaBE = await _CalendarServices.GetCalendarAsync(id);
            return Ok(respuestaBE);
        }


    }
}
