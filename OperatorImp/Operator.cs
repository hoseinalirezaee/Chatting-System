using System;
using System.Collections.Generic;
using System.Text;
using MessageTransfer;
using MessageTransfer.Messages;

namespace OperatorImp
{
    public delegate void MessageReceivedCallback(Message message);

    public class Operator
    {
        public int ID
        {
            get;
            private set;
        }

        private MessageServer messageServer = null;
        private MessageClient messageClient = null;

        public event MessageReceivedCallback MessageReceived;

        public Operator(int id)
        {
            ID = id;
            messageClient = new MessageClient();
            
        }

        public void ConnectToUser(EndPoint serverEndPoint)
        {
            messageServer = new MessageServer();
            messageServer.MessageReceived += MessageServer_MessageReceived;
            messageServer.Start();

            messageClient.Connect(serverEndPoint.ToIPEndPoint());

            ReadyRequest readyRequest = new ReadyRequest
            {
                EndPoint = messageServer.ListeningEndPoint,
                OperatorID = ID
            };

            messageClient.SendMessage(readyRequest);
        }

        private void MessageServer_MessageReceived(Message message, System.Net.IPEndPoint endPoint)
        {
            MessageReceived?.Invoke(message);
        }

        public void SendMessage(Message message)
        {
            messageServer.SendMessage(message);
        }




    }
}
