using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using apiCalendar.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;
using apiCalendar.Services.Contratos;

namespace apiCalendar.Services.Implementaciones
{
    public class EmailServices : IEmailServices
    {

        //private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private static string SERVIDOR = "";
        private static string FROM = "";
        private static string DFROM = "";
        private static string TO = "";
        private static string CC = "";
        private static string CCO = "";
        private const bool ISBODYHTML = true;
        private static string BODY = "";
        private static string SUBJECT = "";
        private static string URL = "";
        private static string ATENCIONCORREO = "";
        private string respuesta = "";
        private string retorno = "";

        public string mensajeResultadoEnvio = "OK";

        private MailPriority HIGH = MailPriority.High;

        // HTMLView para mostrar imágenes automáticamente en correos
        //private AlternateView HTMLView;

        public string Respuesta
        {
            get { return respuesta; }
            set { respuesta = value; }
        }
        public string EmailTO
        {
            get { return TO; }
            set { TO = value; }
        }
        public string EmailAtencionCorreo
        {
            get { return ATENCIONCORREO; }
            set { ATENCIONCORREO = value; }
        }
        public string EmailURL
        {
            get { return URL; }
            set { URL = value; }
        }

        public string EmailFrom
        {
            get { return FROM; }
            set { FROM = value; }
        }

        public string EmailSubject
        {
            get { return SUBJECT; }
            set { SUBJECT = value; }
        }

        public string EmailBody
        {
            get { return BODY; }
            set { BODY = value; }
        }

        public string EmailCC
        {
            get { return CC; }
            set { CC = value; }
        }

        public string EmailCCO
        {
            get { return CCO; }
            set { CCO = value; }
        }

        public string Retorno
        {
            get { return retorno; }
            set { retorno = value; }
        }

        private readonly IConfiguration _config;
        private readonly ILogger<EmailServices> _logger;

        public EmailServices(IConfiguration config, ILogger<EmailServices> logger)
        {
            _config = config;
            _logger = logger;
            retorno = "1";
            SERVIDOR = _config.GetValue<string>("CredencialesEmail:SMTP_CORREO"); //ConfigurationManager.AppSettings["SMTP_CORREO"];
            FROM = _config.GetValue<string>("CredencialesEmail:DE_CORREO"); //ConfigurationManager.AppSettings["DE_CORREO"];
            DFROM = _config.GetValue<string>("CredencialesEmail:NOMBRE_CORREO"); //ConfigurationManager.AppSettings["NOMBRE_CORREO"];
            ATENCIONCORREO = _config.GetValue<string>("CredencialesEmail:ATENCION_CORREO");//ConfigurationManager.AppSettings["ATENCION_CORREO"];
            CCO = _config.GetValue<string>("CredencialesEmail:COPIA_OCULTA"); //ConfigurationManager.AppSettings["COPIA_OCULTA"];
            TO = "";
            CC = "";
            BODY = "";
            SUBJECT = "";
            URL = "";
        }

        private bool disposed = false;

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposed) return;
            if (disposing)
            {
                retorno = null;
                SERVIDOR = null;
                FROM = null;
                DFROM = null;
            }
            disposed = true;
        }
        ~EmailServices()
        {
            Dispose(false);
        }

        public void EnviarEmailCopiaOculta()
        {
            try
            {
                using (MailMessage correo = new MailMessage())
                {
                    correo.From = new MailAddress(FROM, DFROM);
                    correo.To.Add(TO);

                    if (CC != "")
                    {
                        correo.CC.Add(CC);
                    }

                    if (CCO != "")
                    {
                        //string repositorioCorreo = ConfigurationManager.AppSettings["COPIA_OCULTA"];
                        if (EmailCCO != CCO)
                        {
                            correo.Bcc.Add(EmailCCO);
                        }
                        correo.Bcc.Add(CCO);
                    }

                    correo.Subject = SUBJECT;
                    correo.Body = BODY;
                    correo.IsBodyHtml = ISBODYHTML;
                    correo.Priority = HIGH;

                    using (SmtpClient smtp = new SmtpClient())
                    {
                        smtp.Host = SERVIDOR;
                        smtp.Send(correo);
                        _logger.LogInformation("Se realizo el envio de correo correctamente a: " + correo.To + " con copia a: " + correo.CC + " con copia oculta a: " + correo.Bcc);
                    }
                }
            }
            catch (Exception ex)
            {
                mensajeResultadoEnvio = "Error al intentar enviar el correo." + " Detalles: " + ex.Message;
                _logger.LogError(ex, mensajeResultadoEnvio);
            }
        }

    }
}
