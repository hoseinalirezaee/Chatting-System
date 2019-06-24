using System;
using System.Collections.Generic;
using System.Text;

namespace MessageTransfer.Messages
{
    public class ReadyResponse : Message
    {
        public int ActiveSessions { get; set; }
        public int MaxSession { get; set; }

        public override int Type => 6;
    }
}
