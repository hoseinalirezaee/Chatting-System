using System;
using System.Collections.Generic;
using System.Text;

namespace MessageTransfer.Messages
{
    public enum TextDirection
    {
        RightToLeft = 1,
        LeftToRight = 2
    }

    
    public class TextMessage : Message
    {
        public string Text { get; set; }
        public DateTime DateTime { get; set; }
        public TextDirection Direction { get; set; }

        public override int Type => 11;
    }
}
