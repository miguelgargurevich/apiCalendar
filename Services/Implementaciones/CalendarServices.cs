﻿using apiCalendar.Entidades;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Globalization;
using apiCalendar.Services.Contratos;
using apiCalendar.Repository.Contratos;

namespace apiCalendar.Services.Implementaciones
{
    public class CalendarServices : ICalendarServices
    {

        private readonly IConfiguration _config;
        private readonly ICalendarRepository _apiCalendarRepository;
        private readonly ILogger<CalendarServices> _logger;
        IEmailServices _emailServices;

        private string claveAcceso;

        public CalendarServices(IConfiguration config, ICalendarRepository apiCalendarRepository,
            ILogger<CalendarServices> logger, IEmailServices emailServices)
        {
            _config = config;
            _apiCalendarRepository = apiCalendarRepository;
            _logger = logger;
            _emailServices = emailServices;
            claveAcceso = _config.GetValue<string>("Claves:CLAVE_GENERICA");
        }

        #region "Metodos o procedimientos"

        public IEnumerable<EventTypeBE> GetEventTypes()
        {
            IEnumerable<EventTypeBE> list = new List<EventTypeBE>();
            try
            {
                list = _apiCalendarRepository.GetEventTypes();
            }
            catch (Exception ex)
            {
                //list.code = "-1";
                //list.message = "Error: " + ex.Message.ToString();

                CapturarError(ex, "apiCalendarService", "GetEventTypes");
            }

            return list;

        }


        public RespuestaBE ProcesarLineaCredito(CreditLineBE creditLineBE)
        {
            RespuestaBE rptaBE = new RespuestaBE();

            try
            {
                rptaBE = _apiCalendarRepository.ProcesarLineaCredito(creditLineBE);
            }
            catch (Exception ex)
            {
                rptaBE.code = "-1";
                rptaBE.message = "Error: " + ex.Message.ToString();

                CapturarError(ex, "apiCalendarService", "ProcesarLineaCredito");
            }

            return rptaBE;
        }

        public void CapturarError(Exception error, string controlador = "", string accion = "")
        {
            var msg = error.Message;
            if (error.InnerException != null)
            {
                msg = msg + "/;/" + error.InnerException.Message;
                if (error.InnerException.InnerException != null)
                {
                    msg = msg + "/;/" + error.InnerException.InnerException.Message;
                    if (error.InnerException.InnerException.InnerException != null)
                        msg = msg + "/;/" + error.InnerException.InnerException.InnerException.Message;
                }
            }

            var fechahora = DateTime.Now.ToString();
            var comentario = $@"***ERROR: [{fechahora}] [{controlador}/{accion}] - MensajeError: {msg}";
            string errorFormat = string.Format("{0} | {1}", comentario, error.StackTrace);
            _logger.LogError(errorFormat);

        }

        #endregion

    }
}
