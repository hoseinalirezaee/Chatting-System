using System;
using System.Collections.Generic;
using System.Text;

namespace MessageTransfer.Messages
{
    public class Introduction : Message
    {
        public string Name
        {
            get;
            set;
        }
        public override int Type => 14;
    }
}
