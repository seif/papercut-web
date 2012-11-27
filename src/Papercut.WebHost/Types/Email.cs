namespace Papercut.WebHost.Types
{
    using System.Collections.Generic;

    public class Email
    {
        public List<string> To;

        public string From;

        public string Subject;

        public string Body;
    }
}