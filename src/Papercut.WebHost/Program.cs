using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Papercut.WebHost
{
    using System.Configuration;
    using System.IO;
    using System.Net;
    using System.Web;

    using ServiceStack.Common;
    using ServiceStack.Common.Web;
    using ServiceStack.Logging;
    using ServiceStack.Messaging.Rcon;
    using ServiceStack.ServiceHost;
    using ServiceStack.Text;
    using ServiceStack.WebHost.Endpoints;
    using ServiceStack.WebHost.Endpoints.Extensions;
    using ServiceStack.WebHost.Endpoints.Support;

    using Topshelf;

    using HttpRequestWrapper = ServiceStack.WebHost.Endpoints.Extensions.HttpRequestWrapper;
    using HttpResponseWrapper = ServiceStack.WebHost.Endpoints.Extensions.HttpResponseWrapper;

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
                        listener.CatchAllHandlers.Add((httpMethod, pathInfo, filePath) =>{
                            if (pathInfo == "/") return new DefaultDocumentNoRedirectHandler();
                                    return null;
                                });
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

    class DefaultDocumentNoRedirectHandler : IHttpHandler, IServiceStackHttpHandler
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(DefaultDocumentNoRedirectHandler));

        public void ProcessRequest(HttpContext context)
        {
            ProcessRequest(
            new HttpRequestWrapper(null, context.Request),
            new HttpResponseWrapper(context.Response),
            null);
        }

        private DateTime DefaultFileModified { get; set; }
        private string DefaultFilePath { get; set; }
        private byte[] DefaultFileContents { get; set; }

        /// <summary>
        /// Keep default file contents in-memory
        /// </summary>
        /// <param name="defaultFilePath"></param>
        public void SetDefaultFile(string defaultFilePath)
        {
            try
            {
                this.DefaultFileContents = File.ReadAllBytes(defaultFilePath);
                this.DefaultFilePath = defaultFilePath;
                this.DefaultFileModified = File.GetLastWriteTime(defaultFilePath);
            }
            catch (Exception ex)
            {
                log.Error(ex.Message, ex);
            }
        }

        public void ProcessRequest(IHttpRequest request, IHttpResponse response, string operationName)
        {
            response.EndHttpRequest(skipClose: true, afterBody: r =>
            {
                var requestedPath = request.GetPhysicalPath();
                var fi = new FileInfo(requestedPath);
                foreach (var defaultDoc in EndpointHost.Config.DefaultDocuments)
                {
                    var defaultFileName = Path.Combine(fi.FullName, defaultDoc);
                    if (!File.Exists(defaultFileName)) continue;
                    TimeSpan maxAge;
                    if (r.ContentType != null && EndpointHost.Config.AddMaxAgeForStaticMimeTypes.TryGetValue(r.ContentType, out maxAge))
                    {
                        r.AddHeader(HttpHeaders.CacheControl, "max-age=" + maxAge.TotalSeconds);
                    }
                    
                    if (request.HasNotModifiedSince(fi.LastWriteTime))
                    {
                        r.ContentType = MimeTypes.GetMimeType(defaultFileName);
                        r.StatusCode = 304;
                        return;
                    }

                    try
                    {
                        r.AddHeaderLastModified(fi.LastWriteTime);
                        r.ContentType = MimeTypes.GetMimeType(defaultFileName);

                        if (defaultFileName.EqualsIgnoreCase(this.DefaultFilePath))
                        {
                            if (fi.LastWriteTime > this.DefaultFileModified)
                                SetDefaultFile(this.DefaultFilePath); //reload

                            r.OutputStream.Write(this.DefaultFileContents, 0, this.DefaultFileContents.Length);
                            r.Close();
                            return;
                        }

                        if (!Env.IsMono)
                        {
                            r.TransmitFile(defaultFileName);
                        }
                        else
                        {
                            r.WriteFile(defaultFileName);
                        }
                    }
                    catch (Exception ex)
                    {
                        log.ErrorFormat("Static file {0} forbidden: {1}", request.PathInfo, ex.Message);
                        throw new HttpException(403, "Forbidden.");
                    }
                    return;
                }
            });
        }

        public bool IsReusable
        {
            get { return true; }
        }

    }
}
