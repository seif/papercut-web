using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Papercut.WebHost
{
    using System.Configuration;
    using System.Net;

    using ServiceStack.Messaging.Rcon;

    using Topshelf;

    class Program
    {
        static void Main(string[] args)
        {
            HostFactory.Run(x => x.Service<PapercutHttpListener>(c =>
            {
                x.UseLog4Net("log4net.config");

                c.ConstructUsing(s =>
                    {
                        var listener = new PapercutHttpListener();
                        listener.Init();
                        return listener;
                    });
                c.WhenStarted(s =>
                    {
                        var port = int.Parse(ConfigurationManager.AppSettings["Port"]);
                        var host = ConfigurationManager.AppSettings["Host"];
                        s.Start(string.Format("http://{0}:{1}/", host, port));
                    }
                    );
                c.WhenStopped(s =>
                {
                    s.Stop();
                });
            }));
        }
    }
}
