namespace Papercut.WebHost.Operations
{
    using System.Collections.Generic;
    using System.ComponentModel;

    using Papercut.WebHost.Types;

    using ServiceStack.ServiceHost;

    [Description("GET to retrieve all emails\n"
                   + "DELETE to delete all emails\n")]
    [Route("/emails", "GET,DELETE")]
    public class MailboxRequest : IReturn<MailboxResponse>
    {
        public int Page { get; set; }
    }


    public class MailboxResponse
    {
        public MailboxResponse()
        {
            this.Emails = new List<EmailResponse>();
        }

        public List<EmailResponse> Emails { get; set; }

        public List<Link> Links { get; set; }

        public int Page { get; set; }

        public int Pages { get; set; }
    }
}