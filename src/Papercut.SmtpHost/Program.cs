namespace Papercut.SmtpHost
{
    using System;
    using System.Configuration;
    using System.Net;

    using Papercut.Smtp;

    using Topshelf;

    class Program
    {
        static void Main(string[] args)
        {
            HostFactory.Run(x => x.Service<Server>(c => 
            {
                x.UseLog4Net("log4net.config");

                c.ConstructUsing(s =>
                    {
                        var address = IPAddress.Parse(ConfigurationManager.AppSettings["IP"]);
                        var port = int.Parse(ConfigurationManager.AppSettings["Port"]);
                        var mailFolder = ConfigurationManager.AppSettings["MailFolder"];

                        return new Server(address, port, new Processor(mailFolder));
                    });
                c.WhenStarted(s => s.Start());
                c.WhenStopped(s => s.Stop());
            }));
        }
    }
}
