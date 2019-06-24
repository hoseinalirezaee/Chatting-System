using MessageTransfer.Messages;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Xml.Serialization;

namespace MessageTransfer
{
    internal class MessageReceiver
    {
        private TcpClient localSocket = null;
        private NetworkStream stream = null;
        private XmlSerializer serializer = null;
        private StreamReader streamReader = null;

        public MessageReceiver(TcpClient localSocket)
        {
            if (localSocket == null)
                throw new ArgumentNullException("localSocket can not be null.");

            this.localSocket = localSocket;
            this.stream = localSocket.GetStream();
            this.serializer = new XmlSerializer(typeof(Message));
            this.streamReader = new StreamReader(this.stream);
        }

        public Message ReceiveMessage()
        {
            var lineRead = this.streamReader.ReadLine();
            StringReader reader = new StringReader(lineRead);
            var message = serializer.Deserialize(reader) as Message;
            return message;
        }
    }
}
