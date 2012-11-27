namespace Papercut.WebHost.Operations
{
    using System.Collections.Generic;
    using System.ComponentModel;

    using Papercut.WebHost.Types;

    using ServiceStack.ServiceHost;

    [Description("GET the Mailbox at {Name}\n"
               + "POST to create a new mailbox to any {Name} in the /ReadWrite folder\n"
               + "DELETE to delete Mailbox at {Name} in the /ReadWrite folder\n")]
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