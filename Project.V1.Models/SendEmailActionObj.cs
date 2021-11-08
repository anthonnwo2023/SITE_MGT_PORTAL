using System.Collections.Generic;

namespace Project.V1.Models
{
    public class SendEmailActionObj
    {
        public string Name { get; set; }

        public string Title { get; set; }

        public string Greetings { get; set; }

        public string Comment { get; set; }

        public string M2Uname { get; set; }

        public string Link { get; set; }

        public string BodyType { get; set; }

        public string Attachment { get; set; }

        public List<SenderBody> To { get; set; }

        public List<SenderBody> CC { get; set; }
    }

    public class SenderBody
    {
        public string Name { get; set; }

        public string Address { get; set; }
    }
}
