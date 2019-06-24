using System;
using System.Collections.Generic;
using System.Text;

namespace MessageTransfer.Messages
{
    public class StartupResponse : Message
    {
        public Status Status { get; set; }

        public override int Type => 10;
    }
}
