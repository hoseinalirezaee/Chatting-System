using System;
using System.Collections.Generic;
using System.Text;


namespace MessageTransfer.Messages
{
    public class VerificationRequest : Message
    {
        public string UserID { get; set; }
        public EndPoint EndPoint { get; set; }

        public override int Type => 12;
    }
}
