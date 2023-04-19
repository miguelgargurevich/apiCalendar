
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
using apiCalendar.Models;
using Microsoft.AspNetCore.Mvc.Formatters;

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
            var connectionString = _configuration.GetConnectionString("Produccion");
            List<EventTypeBE> list = new List<EventTypeBE>();
            var queryString = "select * from [dbo].[EventType]";

            using (var conn = new SqlConnection(connectionString))
            {
                using (var adapter = new SqlDataAdapter(queryString, conn))
                {
                    conn.Open();
                    var reader = adapter.SelectCommand.ExecuteReader();
                    while (reader.Read())
                    {
                        EventTypeBE obj = new EventTypeBE();
                        obj.Id = reader.GetInt32(0);
                        obj.Name = reader.GetString(1);
                        obj.Color = reader.GetString(2);
                        list.Add(obj);
                    }
                    
                }
            }

            return list;
        }

        public async Task<IEnumerable<EventTypeBE>> GetEventTypesAsync()
        {
            var connectionString = _configuration.GetConnectionString("Produccion");
            List<EventTypeBE> list = new List<EventTypeBE>();
            var queryString = "select * from [dbo].[EventType]";

            using (var connection = new SqlConnection(connectionString))
            {
                using (var adapter = new SqlDataAdapter(queryString, connection))
                {
                    await connection.OpenAsync();
                    var reader = await adapter.SelectCommand.ExecuteReaderAsync();
                    while (await reader.ReadAsync())
                    {
                        EventTypeBE myevent = new EventTypeBE();
                        myevent.Id = reader.GetInt32(0);
                        myevent.Name = reader.GetString(1);
                        myevent.Color = reader.GetString(2);
                        list.Add(myevent);
                    }

                }
            }

            return list.ToArray();
        }

        public async Task<CalendarBE> PostEventAddAsync(CalendarBE calendarBE)
        {
            CalendarBE list = new CalendarBE();
            var _connStr = _configuration.GetConnectionString("Produccion");
            string _query = "INSERT INTO [Calendar] (Title,StartDate,EndDate,AllDay,EventTypeId,EventTypeName,CalendarTypeId,CalendarTypeName,Description,UserCreate,DateCreate) " +
                "values (@title,@startdate,@enddate,@allDay,@eventTypeId,@eventTypeName,@calendarTypeId,@calendarTypeName,@description,@userCreate,@dateCreate)" +
                " Set @id = Scope_Identity();";
            
            using (SqlConnection conn = new SqlConnection(_connStr))
            {
                using (SqlCommand comm = new SqlCommand())
                {
                    comm.Connection = conn;
                    comm.CommandType = CommandType.Text;
                    comm.CommandText = _query;
                    //comm.Parameters.AddWithValue("@id", calendarBE.id);
                    comm.Parameters.AddWithValue("@title", calendarBE.title);
                    comm.Parameters.AddWithValue("@startdate", calendarBE.start);
                    comm.Parameters.AddWithValue("@enddate", calendarBE.end);
                    comm.Parameters.AddWithValue("@allDay", calendarBE.allDay);
                    comm.Parameters.AddWithValue("@eventTypeId", calendarBE.EventTypeId);
                    comm.Parameters.AddWithValue("@eventTypeName", calendarBE.type);
                    comm.Parameters.AddWithValue("@calendarTypeId", calendarBE.CalendarTypeId);
                    comm.Parameters.AddWithValue("@calendarTypeName", calendarBE.CalendarTypeName);
                    comm.Parameters.AddWithValue("@description", calendarBE.description);
                    comm.Parameters.AddWithValue("@userCreate", calendarBE.UserCreate);
                    comm.Parameters.AddWithValue("@dateCreate", DateTime.Now);
                    comm.Parameters.Add("@id", SqlDbType.Int).Direction = ParameterDirection.Output;
                    try
                    {
                        conn.Open();
                        comm.ExecuteNonQuery();

                        int returnID = (int)comm.Parameters["@id"].Value;
                        list.id = returnID;
                    }
                    catch (SqlException ex)
                    {
                        CapturarError(ex, "Repository", "PostEventAddAsync");
                        // other codes here
                        // do something with the exception
                        // don't swallow it.
                    }
                }
            }

            return list;

        }

        public async Task<CalendarBE> PostEventUpdAsync(CalendarBE calendarBE)
        {
            CalendarBE list = new CalendarBE();
            var _connStr = _configuration.GetConnectionString("Produccion");
            string _query = "" +
                "UPDATE Calendar SET title = @title, startdate = @startdate, enddate = @enddate, description = @description, eventtypeid = @eventtypeid, eventtypename = @eventtypename, calendartypeid = @calendartypeid,calendartypename = @calendartypename, allday = @allday WHERE id = @id";
            using (SqlConnection conn = new SqlConnection(_connStr))
            {
                using (SqlCommand comm = new SqlCommand())
                {
                    comm.Connection = conn;
                    comm.CommandType = CommandType.Text;
                    comm.CommandText = _query;
                    comm.Parameters.AddWithValue("@id", calendarBE.id);
                    comm.Parameters.AddWithValue("@title", calendarBE.title);
                    comm.Parameters.AddWithValue("@startdate", calendarBE.start);
                    comm.Parameters.AddWithValue("@enddate", calendarBE.end);
                    comm.Parameters.AddWithValue("@allDay", calendarBE.allDay);
                    comm.Parameters.AddWithValue("@eventTypeId", calendarBE.EventTypeId);
                    comm.Parameters.AddWithValue("@eventTypeName", calendarBE.type);
                    comm.Parameters.AddWithValue("@calendarTypeId", calendarBE.CalendarTypeId);
                    comm.Parameters.AddWithValue("@calendarTypeName", calendarBE.CalendarTypeName);
                    comm.Parameters.AddWithValue("@description", calendarBE.description);
                    try
                    {
                        conn.Open();
                        comm.ExecuteNonQuery();
                    }
                    catch (SqlException ex)
                    {
                        CapturarError(ex, "Repository", "PostEventUpdAsync");
                        // other codes here
                        // do something with the exception
                        // don't swallow it.
                    }
                }
            }

            return list;

        }

        public async Task<CalendarBE> PostEventDelAsync(CalendarBE calendarBE)
        {
            CalendarBE list = new CalendarBE();
            var _connStr = _configuration.GetConnectionString("Produccion");
            string _query = "" +
                "Delete from Calendar WHERE id = @id";
            using (SqlConnection conn = new SqlConnection(_connStr))
            {
                using (SqlCommand comm = new SqlCommand())
                {
                    comm.Connection = conn;
                    comm.CommandType = CommandType.Text;
                    comm.CommandText = _query;
                    comm.Parameters.AddWithValue("@id", calendarBE.id);
                   
                    try
                    {
                        conn.Open();
                        comm.ExecuteNonQuery();
                    }
                    catch (SqlException ex)
                    {
                        CapturarError(ex, "Repository", "PostEventDelAsync");
                        // other codes here
                        // do something with the exception
                        // don't swallow it.
                    }
                }
            }

            return list;

        }

        public async Task<IEnumerable<CalendarBE>> GetCalendarAsync(int id)
        {
            List<CalendarBE> list = new List<CalendarBE>();
            var connectionString = _configuration.GetConnectionString("Produccion");

            var idquery = "a.id";
            if (id != 0)
                idquery = id.ToString();

            string queryString = "SELECT a.id, a.title, a.startdate, a.enddate, a.description, a.eventtypeid, a.eventtypename, a.calendartypeid, " +
                "a.calendartypename, a.userCreate, a.dateCreate, a.allday, b.color " +
                "FROM  dbo.Calendar a inner join dbo.EventType b on b.name = a.eventtypename where a.id = " + idquery;

            try {
                using (var conn = new SqlConnection(connectionString))
                {
                    using (var adapter = new SqlDataAdapter(queryString, conn))
                    {
                        conn.Open();
                        var reader = adapter.SelectCommand.ExecuteReader();
                        while (reader.Read())
                        {
                            CalendarBE obj = new CalendarBE();
                            obj.id = reader.GetInt32(0);
                            obj.title = reader.GetString(1);
                            obj.start = reader.GetDateTime(2);
                            obj.end = reader.GetDateTime(3);
                            obj.description = reader.GetString(4);
                            obj.EventTypeId = reader.GetInt32(5);
                            obj.type = reader.GetString(6);
                            obj.CalendarTypeId = reader.GetInt32(7);
                            obj.CalendarTypeName = reader.GetString(8);
                            obj.UserCreate = reader.GetInt32(9);
                            obj.DateCreate = reader.GetDateTime(10);
                            obj.allDay = Convert.ToBoolean(reader.GetInt32(11));
                            obj.color = reader.GetString(12);
                            list.Add(obj);
                        }

                    }
                }
            }
            catch (SqlException ex) 
            { 
                CapturarError(ex, "Repository", "GetCalendarAsync");
            }
           

            return list;

        }
        #endregion

        #region "Metodos y Funciones Linea de credito"

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
