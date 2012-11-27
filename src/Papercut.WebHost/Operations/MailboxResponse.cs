namespace Papercut.WebHost.Operations
{
    using Papercut.WebHost.Types;

    using ServiceStack.ServiceInterface.ServiceModel;

    public class MailboxResponse : IHasResponseStatus
    {
        public MailboxResult Mailbox { get; set; }
        public ResponseStatus ResponseStatus { get; set; }
    }
}