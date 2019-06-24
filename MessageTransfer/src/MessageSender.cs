using MessageTransfer.Messages;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace MessageTransfer
{
    internal class MessageSender
    {
        private TcpClient localSocket = null;
        private NetworkStream stream = null;
        private XmlSerializer serializer = null;
        private StreamWriter streamWriter = null;

        public MessageSender(TcpClient localSocket)
        {
            if (localSocket == null)
                throw new ArgumentNullException("localSocket can not be null.");

            this.localSocket = localSocket;
            this.stream = localSocket.GetStream();
            this.serializer = new XmlSerializer(typeof(Message));
            this.streamWriter = new StreamWriter(this.stream);
        }

        public void SendMessage(Message message)
        {
            StringBuilder stringBuilder = new StringBuilder();
            XmlWriterSettings xmlWriterSettings = new XmlWriterSettings
            {
                NewLineHandling = NewLineHandling.None
            };
            XmlWriter writer = XmlWriter.Create(stringBuilder, xmlWriterSettings);

            serializer.Serialize(writer, message);
            writer.Close();
            var stringToSend = stringBuilder.ToString();
            streamWriter.WriteLine(stringToSend);
            streamWriter.Flush();
        }
    }
}
