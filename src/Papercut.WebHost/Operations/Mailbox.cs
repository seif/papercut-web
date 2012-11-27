namespace Papercut.WebHost.Operations
{
    using System.Collections.Generic;
    using System.ComponentModel;

    using Papercut.WebHost.Types;

    using ServiceStack.ServiceHost;

    [Description("GET the Mailbox at {Name}\n"
               + "POST to create a new mailbox to any {Name} in the /ReadWrite folder\n"
               + "DELETE to delete Mailbox at {Name} in the /ReadWrite folder\n")]
    [Route("/mailboxes/{Name}")]	
    public class Mailbox : IReturn<MailboxResult>
    {
        public string Name { get; set; }
    }

    [Route("/mailboxes", "GET")]
    public class AllMailboxes : IReturn<List<Mailbox>>
    {
    }
}