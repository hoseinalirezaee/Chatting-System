using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;

namespace MessageTransfer
{
    internal class ClientInfo
    {
        public TcpClient ClientSocket { get; set; }
        public MessageSender MessageSender { get; set; }
        public MessageReceiver MessageReceiver { get; set; }
    }
}
