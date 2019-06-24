using System;
using System.Collections.Generic;
using System.Text;
using MessageTransfer;

namespace ServerImp
{
    public class Operator
    {
        public int OperatorID { get; set; }
        public EndPoint EndPoint { get; set; }
        public EndPoint ListeningEndPoint { get; set; }
        public int State { get; set; }
    }
}
