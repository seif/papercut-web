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

        public List<Mailbox> Get(Mailboxes request)
        {
            var mailboxes =
                Directory.GetDirectories(this.Config.MailFolder).Select(m =>
                    {
                        string name = new DirectoryInfo(m).Name;
                        return new Mailbox() { Name = name, Links = new List<Link>(new[] { this.GetMailboxLink(name) }) };
                    }).ToList();

            return mailboxes;
        }

        public MailboxResponse Get(Mailbox request)
        {
            var mailboxPath = new DirectoryInfo(Path.Combine(this.Config.MailFolder, request.Name));
            ValidateExists(request.Name, mailboxPath);

            string[] emails = Directory.GetFiles(mailboxPath.FullName, "*.eml");

            var response = new MailboxResponse() { Name = request.Name, Links = new List<Link>(new[] { this.GetMailboxLink(request.Name) }) };

            foreach (var entry in emails)
            {
                var mailMessage = GetMailMessage(entry);

                response.Emails.Add(new EmailResponse()
                    {
                        Body = mailMessage.Body,
                        Subject = mailMessage.Subject,
                        To = mailMessage.To.Select(m => m.Address).ToList(),
                        From = mailMessage.From.Address,
                        Links = new List<Link>(new [] { this.GetEmailLink(request.Name, entry) })
                    });
            }
        
            return response;
        }

        public MailboxResponse Post(Mailbox request)
        {
            var mailboxPath = new DirectoryInfo(Path.Combine(this.Config.MailFolder, request.Name));
            if(mailboxPath.Exists)
                throw new HttpError(HttpStatusCode.Conflict, new NotSupportedException("Mailbox already exists: " + request.Name));

            mailboxPath.Create();

            return new MailboxResponse(){Name = request.Name};
        }

        public MailboxResponse Delete(Mailbox request)
        {
            var mailboxPath = new DirectoryInfo(Path.Combine(this.Config.MailFolder, request.Name));
            ValidateExists(request.Name, mailboxPath);

            Directory.Delete(mailboxPath.FullName, true);

            return new MailboxResponse() { Name = request.Name, Links = new List<Link>(new[] { this.GetMailboxLink(request.Name) }) };
        }

        public EmailResponse Get(Email request)
        {
            var mailboxPath = new DirectoryInfo(Path.Combine(this.Config.MailFolder, request.Mailbox));
            ValidateExists(request.Mailbox, mailboxPath);
            var emailPath = new FileInfo(Path.Combine(mailboxPath.FullName, request.Id));
            ValidateExists(request.Id, emailPath);

            var emailEx = GetMailMessage(emailPath.FullName);
            
            return new EmailResponse()
                {
                    Body = emailEx.Body,
                    From = emailEx.From.Address,
                    Subject = emailEx.Subject,
                    To = emailEx.To.Select(t => t.Address).ToList(),
                    Links = new List<Link>(new[] { this.GetEmailLink(request.Mailbox, emailPath.FullName) })
                };
        }

        public EmailResponse Delete(Email request)
        {
            var mailboxPath = new DirectoryInfo(Path.Combine(this.Config.MailFolder, request.Mailbox));
            ValidateExists(request.Mailbox, mailboxPath);
            var emailPath = new FileInfo(Path.Combine(mailboxPath.FullName, request.Id));
            ValidateExists(request.Id, emailPath);

            var emailEx = GetMailMessage(emailPath.FullName);

            File.Delete(emailPath.FullName);

            return new EmailResponse()
            {
                Body = emailEx.Body,
                From = emailEx.From.Address,
                Subject = emailEx.Subject,
                To = emailEx.To.Select(t => t.Address).ToList(),
                Links = new List<Link>(new[] { this.GetEmailLink(request.Mailbox, emailPath.FullName) })
            };
        }


        private Link GetEmailLink(string mailbox, string entry)
        {
            return new Link { Href = string.Format("mailboxes/{0}/{1}", mailbox, new FileInfo(entry).Name), Rel = "self" };
        }

        private Link GetMailboxLink(string mailbox)
        {
            return new Link { Href = string.Format("mailboxes/{0}/", mailbox), Rel = "self" };
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