using System;
using System.Collections.Generic;
using System.Text;

namespace MessageTransfer.Messages
{
    public class FileMessage : Message
    {
        
        public string FileName { get; set; }
        public ulong Size { get; set; }
        public int ID { get; set; }

        public override int Type => 1;
    }
}
