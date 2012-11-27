namespace Papercut.WebHost.Types
{
    using System.Collections.Generic;

    using ServiceStack.ServiceHost;

    [Route("/mailboxes/{Mailbox}/{Id}", "GET,DELETE")]
    public class Email : IReturn<EmailResponse>
    {
        public string Mailbox { get; set; }

        public string Id { get; set; }
    }

    public class EmailResponse
    {
        public EmailResponse()
        {
            Links = new List<Link>();
        }

        public List<string> To { get; set; }

        public string From { get; set; }

        public string Subject { get; set; }

        public string Body { get; set; }

        public List<Link> Links { get; set; }
    }

    public class Link
    {
        public string Rel { get; set; }
        public string Href { get; set; }
    }
}