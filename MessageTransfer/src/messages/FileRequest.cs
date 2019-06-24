using System;
using System.Collections.Generic;
using System.Text;
using MessageTransfer;

namespace MessageTransfer.Messages
{
    public class FileRequest : Message
    {
        public int ID { get; set; }
        public EndPoint RecieverEndPoint { get; set; }

        public override int Type => 2;
    }
}
