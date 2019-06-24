using System;
using System.Collections.Generic;
using System.Text;

namespace MessageTransfer.Messages
{
    public class P2PRequest : Message
    {
        public string UserName { get; set; }
        public string UserEmail { get; set; }
        public string UserID { get; set; }

        public override int Type => 3;
    }


}
