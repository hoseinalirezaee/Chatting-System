using System;
using System.Collections.Generic;
using System.Text;

namespace MessageTransfer.Messages
{
    public class ReadyRequest : Message
    {
        public int OperatorID { get; set; }
        public EndPoint EndPoint { get; set; }

        public override int Type => 5;
    }
}
