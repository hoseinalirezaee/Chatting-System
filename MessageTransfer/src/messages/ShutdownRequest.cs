using System;
using System.Collections.Generic;
using System.Text;

namespace MessageTransfer.Messages
{
    public class ShutdownRequest : Message
    {
        public int OperatorID { get; set; }

        public override int Type => 7;
    }
}
