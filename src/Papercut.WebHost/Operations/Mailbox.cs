namespace Papercut.WebHost.Operations
{
    using System.ComponentModel;

    using ServiceStack.ServiceHost;

    [Description("GET the Mailbox at {Name}\n"
               + "POST to create a new mailbox to any {Name} in the /ReadWrite folder\n"
               + "DELETE to delete Mailbox at {Name} in the /ReadWrite folder\n")]
    [Route("/mailbox")]
    [Route("/mailbox/{Name*}")]	
    public class Mailbox
    {
        public string Name { get; set; }
    }
}