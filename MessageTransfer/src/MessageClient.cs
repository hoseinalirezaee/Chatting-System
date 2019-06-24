using MessageTransfer.Messages;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using MessageTransfer.Exceptions;

namespace MessageTransfer
{
    public class MessageClient
    {
        private IPEndPoint remoteEndPoint = null;
        private TcpClient clientSocket = null;
        private MessageSender sender = null;
        private MessageReceiver receiver = null;
        private Thread receiveThread = null;

        public event MessageReceivedCallback MessageReceived;

        public void Connect(IPEndPoint remoteEndPoint)
        {
            this.remoteEndPoint = remoteEndPoint;
            this.clientSocket = new TcpClient();
            this.clientSocket.Connect(this.remoteEndPoint);
            this.sender = new MessageSender(this.clientSocket);
            this.receiver = new MessageReceiver(clientSocket);

            ThreadStart setting = new ThreadStart(ReceiveMessage);
            this.receiveThread = new Thread(setting);
            receiveThread.Start();
        }

        public void SendMessage(Message message)
        {
            this.sender.SendMessage(message);
        }

        private void ReceiveMessage()
        {
            List<Socket> list = new List<Socket>();
            while (true)
            {
                while (clientSocket == null) ;
                list.Clear();
                list.Add(this.clientSocket.Client);
                Socket.Select(list, null, null, -1);

                if (list[0].Available == 0)
                    throw new RemoteClientShutDown();

                var message = receiver.ReceiveMessage();
                MessageReceived?.Invoke(message, clientSocket.Client.RemoteEndPoint as IPEndPoint);
            }
        }
    }
}
