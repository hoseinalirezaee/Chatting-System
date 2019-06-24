using System;
using System.Collections.Generic;
using System.Text;

namespace MessageTransfer.Messages
{
    public class P2PResponse : Message
    {
        public EndPoint ListeningEndPoint { get; set; }
        public Status Status { get; set; }

        public override int Type => 4;
    }
}
