namespace Papercut.WebHost
{
    using System.Configuration;

    using Funq;

    using ServiceStack.WebHost.Endpoints;

    public class PapercutHttpListener
        : AppHostHttpListenerBase
    {
        private string listeningOn;

        public PapercutHttpListener()
            : base("HttpListener Host for Papercut", typeof(MailboxService).Assembly) { }

        public AppConfig Config { get; set; }

        public string ListeningOn
        {
            get
            {
                return this.listeningOn;
            }
        }

        public override void Configure(Container container)
        {
            this.Config = new AppConfig
                {
                    MailFolder = ConfigurationManager.AppSettings["MailFolder"]
                };

            container.Register(this.Config);

            //this.Routes.Add<Mailbox>("/mailbox")
            //    .Add<Mailbox>("/mailbox/{Name}");
        }

        public override void Start(string urlBase)
        {
            listeningOn = urlBase;
            base.Start(urlBase);
        }
    }
}