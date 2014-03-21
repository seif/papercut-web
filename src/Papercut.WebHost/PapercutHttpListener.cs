using System;

namespace Papercut.WebHost
{
    using System.Configuration;

    using Funq;

    using Papercut.Smtp;

    using ServiceStack.WebHost.Endpoints;

    public class PapercutHttpListener
        : AppHostHttpListenerBase
    {
        private string listeningOn;

        public PapercutHttpListener()
            : base("HttpListener Host for Papercut", typeof(MailboxService).Assembly) { }

        public AppConfig PapercutConfig { get; set; }

        public string ListeningOn
        {
            get
            {
                return this.listeningOn;
            }
        }

        public override void Configure(Container container)
        {
            ServiceStack.Text.JsConfig.DateHandler = ServiceStack.Text.JsonDateHandler.ISO8601;


            string[] exts = { "png", "woff", "ttf", "svg" };
            foreach (string ext in exts)
                EndpointHostConfig.Instance.AllowFileExtensions.Add(ext);

            Plugins.Add(new ServiceStack.Razor.RazorFormat());

            this.PapercutConfig = new AppConfig
                {
                    MailFolder = ConfigurationManager.AppSettings["MailFolder"],
                    EmailsPerPage = Convert.ToInt32(ConfigurationManager.AppSettings["EmailsPerPage"])
                };

            container.Register(this.PapercutConfig);
        }

        public override void Start(string urlBase)
        {
            listeningOn = urlBase;
            Logger.Write("Server Ready - Listening for new connections " + listeningOn + "...");
            base.Start(urlBase);
        }
    }
}