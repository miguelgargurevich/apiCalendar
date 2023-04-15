
using apiCalendar.Entidades;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using apiCalendar.Repository.Contratos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.Data.SqlClient;
using System.Data.Common;
using System.Data;

namespace apiCalendar.Repository.Implementaciones
{

    public class CalendarRepository : ICalendarRepository
    {

        private readonly IConfiguration _configuration;
        private readonly ILogger<CalendarRepository> _logger;
        private readonly IWebHostEnvironment _webHostEnvironment;
        
        int maxNumbersRequest = 3;
        int maxTotalMinutesRequest = 2;

        public CalendarRepository(IConfiguration configuration, ILogger<CalendarRepository> logger, IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
            _configuration = configuration;
            _logger = logger;
        }

        #region "Metodos y Funciones"

        public IEnumerable<EventTypeBE> GetEventTypes() 
        { 
            List<EventTypeBE> list = new List<EventTypeBE>();
            var connectionString = UI.connectionString;
            var queryString = "select * from [dbo].[EventType]";

            using (var connection = new SqlConnection(connectionString))
            {
                using (var adapter = new SqlDataAdapter(queryString, connection))
                {
                    connection.Open();
                    var reader = adapter.SelectCommand.ExecuteReader();
                    while (reader.Read())
                    {
                        EventTypeBE myevent = new EventTypeBE();
                        myevent.id = reader.GetInt32(0);
                        myevent.name = reader.GetString(1);
                        myevent.color = reader.GetString(2);
                        list.Add(myevent);
                    }
                    
                }
            }

            return list.ToArray();
        }
        #endregion

        #region "Metodos y Funciones Linea de credito"
        public RespuestaBE ProcesarLineaCredito(CreditLineBE creditLineBE)
        {
            RespuestaBE respuestaBE = new RespuestaBE();


            try
            {
                //obtiene lastRequest
                bool lastRequestAccepted = true;
                int lastRequestNumbers = 1;
                TimeSpan lastRequestDate = DateTime.Now.TimeOfDay; ;
                TimeSpan nowRequestDate = Convert.ToDateTime(creditLineBE.requestedDate).TimeOfDay;

                var lastTrama = LeerTramas();
                if (lastTrama.requestNumbers != null)
                {
                    lastRequestNumbers = (int)lastTrama.requestNumbers + 1;
                    lastRequestDate = Convert.ToDateTime(lastTrama.requestedDate).TimeOfDay;
                    lastRequestAccepted = Convert.ToBoolean(lastTrama.requestAccepted);
                }

                if (ValidPreviousRequest(lastRequestAccepted, nowRequestDate, lastRequestDate, respuestaBE) == false)
                    return respuestaBE;

                //calcula Recommended apiCalendar
                double recommendedapiCalendar = GetRecommendedapiCalendar(creditLineBE);

                //valida accepted o rejected
                bool accepted = recommendedapiCalendar > creditLineBE.requestedapiCalendar;
                if (accepted)
                {
                    var timeRequest = nowRequestDate - lastRequestDate;

                    if (ValidAcceptedRequest(lastRequestNumbers, timeRequest, respuestaBE) == false)
                        return respuestaBE;
                    else if (lastRequestNumbers >= maxNumbersRequest && timeRequest.TotalMinutes > maxTotalMinutesRequest)
                        lastRequestNumbers = ResetRequestNumbers();

                    var crediLine = Math.Round(recommendedapiCalendar, 2);
                    string msg = "Credit line authorized: " + crediLine.ToString();
                    respuestaBE.code = "1";
                    respuestaBE.status = StatusCodes.Status200OK;
                    respuestaBE.retornoBool = true;
                    respuestaBE.message = "Acepted";
                    respuestaBE.messageDetail = msg;

                    creditLineBE.recommendedapiCalendar = crediLine;
                    creditLineBE.requestAccepted = accepted.ToString();
                    creditLineBE.requestNumbers = lastRequestNumbers;
                    creditLineBE.requestMessage = msg;
                    //guardar Request
                    GuardarTrama(creditLineBE);
                }
                else
                {
                    if (lastRequestAccepted == true)
                        lastRequestNumbers = ResetRequestNumbers();

                    ValidRejectedRequest(lastRequestAccepted, lastRequestNumbers, respuestaBE);

                    var crediLine = Math.Round(recommendedapiCalendar, 2);
                    string msg = respuestaBE.messageDetail;
                    creditLineBE.recommendedapiCalendar = crediLine;
                    creditLineBE.requestAccepted = accepted.ToString();
                    creditLineBE.requestNumbers = lastRequestNumbers;
                    creditLineBE.requestMessage = msg;

                    //guardar Request
                    GuardarTrama(creditLineBE);
                }

                return respuestaBE;

            }
            catch (Exception ex)
            {
                CapturarError(ex, "Repository", "ProcesarLineaCredito");

                respuestaBE.code = "-1";
                respuestaBE.status = StatusCodes.Status500InternalServerError;
                respuestaBE.retornoBool = false;
                respuestaBE.message = "Error";
                respuestaBE.messageDetail = ex.Message;

                return respuestaBE;
            }
        }

        private int ResetRequestNumbers()
        {
            return 1;
        }

        private void ValidRejectedRequest(bool lastRequestAccepted, int lastRequestNumbers, RespuestaBE respuestaBE)
        {
            //After failing 3 times, return the message "A sales agent will contact you".
            if (lastRequestAccepted == false && lastRequestNumbers > 3)
            {
                respuestaBE.code = "-101";
                respuestaBE.status = StatusCodes.Status200OK;
                respuestaBE.retornoBool = false;
                respuestaBE.message = "Rejected";
                respuestaBE.messageDetail = "A sales agent will contact you";
            }
            else
            {
                respuestaBE.code = "-100";
                respuestaBE.status = StatusCodes.Status200OK;
                respuestaBE.retornoBool = false;
                respuestaBE.message = "Rejected";
                respuestaBE.messageDetail = "The recommended credit line is lower than requested";
            }

        }

        private bool ValidAcceptedRequest(int lastRequestNumbers, TimeSpan timeRequest, RespuestaBE respuestaBE)
        {
            //If the system receives 3 or more requests within two minutes, return the http code 429.
            if (lastRequestNumbers >= maxNumbersRequest && timeRequest.TotalMinutes <= maxTotalMinutesRequest)
            {
                respuestaBE.code = "-102";
                respuestaBE.status = StatusCodes.Status429TooManyRequests;
                respuestaBE.retornoBool = false;
                respuestaBE.message = "Error";
                respuestaBE.messageDetail = "The system receives 3 or more requests within two minutes";

                return false;
            }
            return true;

        }

        private bool ValidPreviousRequest(bool lastRequestAccepted, TimeSpan nowRequestDate, TimeSpan lastRequestDate, RespuestaBE respuestaBE)
        {
            //Don't allow a new application requests within 30 seconds next to the previous one, if so,
            //return HTTP code 429.
            if (lastRequestAccepted == false)
            {
                var timeRequest = nowRequestDate - lastRequestDate;
                var timeLimit = Convert.ToDateTime("01/01/2020 00:00:30").TimeOfDay;
                if (timeRequest <= timeLimit)
                {
                    respuestaBE.code = "-102";
                    respuestaBE.status = StatusCodes.Status429TooManyRequests;
                    respuestaBE.retornoBool = false;
                    respuestaBE.message = "Error";
                    respuestaBE.messageDetail = "Don't allow a new application requests within 30 seconds next to the previous one";

                    return false;
                }
            }
            return true;

        }

        private double GetRecommendedapiCalendar(CreditLineBE apiCalendarBE)
        {
            //calcula recommendedapiCalendar
            var recommendedapiCalendar = 0.0;
            if (apiCalendarBE.foundingType == "SME")
            {
                recommendedapiCalendar = apiCalendarBE.monthlyRevenue / 5; //One fifth of the monthly revenue(5:1 ratio)
            }
            else if (apiCalendarBE.foundingType == "Startup")
            {
                var mR = apiCalendarBE.monthlyRevenue / 5;
                var cB = apiCalendarBE.cashBalance / 3; //One third of the cash balance(3:1 ratio)
                var maxValue = Math.Max(mR, cB);
                recommendedapiCalendar = maxValue;
            }
            return recommendedapiCalendar;
        }

        private void GuardarTrama(CreditLineBE apiCalendarBE)
        {

            string contentRootPath = _webHostEnvironment.ContentRootPath;
            string hoy = DateTime.Today.ToString("yyyyMMdd");
            string fileUnicoName = string.Concat(hoy, ".txt");
            string fileUnicoPath = Path.Combine(contentRootPath, "Tramas", fileUnicoName);

            string trama = JsonConvert.SerializeObject(apiCalendarBE);

            File.AppendAllLines(fileUnicoPath, new string[] { trama });

        }

        private CreditLineBE LeerTramas()
        {
            CreditLineBE apiCalendarBE = new CreditLineBE();
            string contentRootPath = _webHostEnvironment.ContentRootPath;
            string hoy = DateTime.Today.ToString("yyyyMMdd");
            string fileUnicoName = string.Concat(hoy, ".txt");
            string fileUnicoPath = Path.Combine(contentRootPath, "Tramas", fileUnicoName);


            bool result = File.Exists(fileUnicoPath);
            if (result == false)
            {
                File.CreateText(fileUnicoPath).Close(); ;
            }
            else
            {
                var line = File.ReadLines(fileUnicoPath);

                if (line.Count() > 0)
                {
                    var lastLine = line.Last();
                    if (!string.IsNullOrEmpty(lastLine))
                        apiCalendarBE = JsonConvert.DeserializeObject<CreditLineBE>(lastLine);
                }
            }

            return apiCalendarBE;
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

    public static class UI
    {
        public static string? connectionString { get; set; }
    }

}
