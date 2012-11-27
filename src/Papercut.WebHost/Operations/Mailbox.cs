namespace Papercut.WebHost.Operations
{
    using System.Collections.Generic;
    using System.ComponentModel;

    using Papercut.WebHost.Types;

    using ServiceStack.ServiceHost;

    [Description("GET to retrieve the mail box\n"
               + "POST to create a new mailbox\n"
               + "DELETE to delete Mailbox\n")]
    [Route("/mailboxes/{Name}", "GET,POST,DELETE")]	
    public class Mailbox : IReturn<MailboxResponse>
    {
        public string Name { get; set; }

        public List<Link> Links { get; set; }
    }

    [Route("/mailboxes", "GET")]
    public class Mailboxes : IReturn<List<Mailbox>>
    {
    }


    public class MailboxResponse
    {
        public MailboxResponse()
        {
            this.Emails = new List<EmailResponse>();
        }

        public List<EmailResponse> Emails { get; set; }

        public List<Link> Links { get; set; }

        public string Name { get; set; }
    }
}