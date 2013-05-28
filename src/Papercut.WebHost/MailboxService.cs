using System.Web;

namespace Papercut.WebHost
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net;

    using Papercut.Smtp.Mime;
    using Papercut.WebHost.Operations;
    using Papercut.WebHost.Types;

    using ServiceStack.Common.Web;
    using ServiceStack.ServiceInterface;
    using ServiceStack.ServiceInterface.ServiceModel;

    public class MailboxService : Service
    {
        public AppConfig Config { get; set; }

        public MailboxResponse Get(MailboxRequest request)
        {
            var mailboxPath = new DirectoryInfo(this.Config.MailFolder);
            
            request.Page = request.Page == 0 ? 1 : request.Page; 

            var emails = mailboxPath.GetFiles("*.eml");
            var numPages = (int)Math.Ceiling(emails.Count() / ((double)Config.EmailsPerPage));

            var emailsToRead = emails
                .OrderByDescending(x => x.LastWriteTimeUtc)
                .Skip(Config.EmailsPerPage * (request.Page - 1))
                .Take(Config.EmailsPerPage)
                .Select(x => x.FullName);

            var response = new MailboxResponse()
            {
                Links = new List<Link>(new[]
                {
                    this.GetNextPageLink(request.Page, numPages),
                    this.GetPreviousPageLink(request.Page)
                }),
                Page = request.Page,
                Pages = numPages
            };

            foreach (var entry in emailsToRead)
            {
				try {
					var mailMessage = GetMailMessage(entry);

					response.Emails.Add(new EmailResponse()
					                    {
					                    	Id = Path.GetFileNameWithoutExtension(entry),
					                    	Body = mailMessage.Body,
					                    	Subject = mailMessage.Subject,
					                    	To = mailMessage.To.Select(m => m.Address).ToList(),
					                    	From = mailMessage.From.Address,
					                    	Date = mailMessage.DeliveryDate,
					                    	Links = new List<Link>(new[] {this.GetEmailLink(entry)})
					                    });
				} catch(Exception e) {
					response.Emails.Add(new EmailResponse()
					                    {
											Id = Path.GetFileNameWithoutExtension(entry),
											Body = e.Message,
											Subject = "[Error reading email]",
											From = "-",
											To = new List<string>()
					                    });
				}
            }
        
            return response;
        }

        public EmailResponse Get(Email request)
        {
            var mailboxPath = new DirectoryInfo(this.Config.MailFolder);
            string emailFileName = HttpUtility.UrlDecode(request.Id);
            var emailPath = new FileInfo(Path.Combine(mailboxPath.FullName, emailFileName + ".eml"));
            ValidateExists(emailFileName, emailPath);

			try {
				var emailEx = GetMailMessage(emailPath.FullName);

				return new EmailResponse()
				       {
                        Id = emailFileName,
				       	Body = emailEx.Body,
				       	From = emailEx.From.Address,
				       	Subject = emailEx.Subject,
				       	Date = emailEx.DeliveryDate,
				       	To = emailEx.To.Select(t => t.Address).ToList(),
				       	Links = new List<Link>(new[] {this.GetEmailLink(emailPath.FullName)})
				       };
			}catch(Exception e) {
				return new EmailResponse()
				       {
                        Id = emailFileName,
				       	Body = "[Email could not be loaded]",
				       	Subject = "[Error reading email]",
				       	From = "-",
				       	To = new List<string>()
				       };

			}
        }

        public EmailResponse Delete(Email request)
        {
            var mailboxPath = new DirectoryInfo(this.Config.MailFolder);
            var emailPath = new FileInfo(Path.Combine(mailboxPath.FullName, request.Id));
            ValidateExists(request.Id, emailPath);

            var emailEx = GetMailMessage(emailPath.FullName);

            File.Delete(emailPath.FullName);

            return new EmailResponse()
            {
                Body = emailEx.Body,
                From = emailEx.From.Address,
                Subject = emailEx.Subject,
                Date = emailEx.DeliveryDate,
                To = emailEx.To.Select(t => t.Address).ToList(),
                Links = new List<Link>(new[] { this.GetEmailLink(emailPath.FullName) })
            };
        }

        public void Delete(MailboxRequest request)
        {
            var mailboxPath = new DirectoryInfo(this.Config.MailFolder);

            foreach (var file in mailboxPath.GetFiles("*.eml"))
            {
                file.Delete();
            }
        }

        private Link GetEmailLink(string entry)
        {
            return new Link { Href = new FileInfo(entry).Name, Rel = "self" };
        }

        private Link GetPreviousPageLink(int currentPage)
        {
            return GetPageNavigationLink(currentPage == 1 ? 1 : --currentPage, "previous");
        }

        private Link GetNextPageLink(int currentPage, int numPages)
        {
            return GetPageNavigationLink(currentPage == numPages ? currentPage : ++currentPage, "next");
        }

        private Link GetPageNavigationLink(int page, string rel)
        {
            return new Link {Href = string.Format("?page={0}", page), Rel = rel};
        }
        
        private static MailMessageEx GetMailMessage(string file)
        {
            var allLines = File.ReadAllLines(file);
            var mimeReader = new MimeReader(allLines);
            return mimeReader.CreateMimeEntity().ToMailMessageEx();
        }

        private static void ValidateExists(string requestPath, FileSystemInfo mailboxPath)
        {
            if (!File.Exists(mailboxPath.FullName) && !Directory.Exists(mailboxPath.FullName))
            {
                throw new HttpError(HttpStatusCode.NotFound, new FileNotFoundException("Could not find: " + requestPath));
            }
        }
    }
}