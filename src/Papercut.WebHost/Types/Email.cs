namespace Papercut.WebHost.Types
{
    using System.Collections.Generic;

    public class Email
    {
        public List<string> To { get; set; }

        public string From { get; set; }

        public string Subject { get; set; }

        public string Body { get; set; }
    }
}