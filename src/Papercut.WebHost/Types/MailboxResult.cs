namespace Papercut.WebHost.Types
{
    using System.Collections.Generic;

    public class MailboxResult
    {
        public MailboxResult()
        {
            this.Emails = new List<Email>();
        }

        public List<Email> Emails { get; set; }

        public string Name { get; set; }
    }
}