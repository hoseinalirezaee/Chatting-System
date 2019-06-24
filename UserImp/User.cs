using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using MessageTransfer;
using MessageTransfer.Messages;


namespace UserImp
{

    public delegate void MessageReceivedCallback(Message message);

    public class User
    {
        public string Name
        {
            get;
            private set;
        }
        public string Email
        {
            get;
            private set;
        }
        public string ID
        {
            get;
            private set;
        }

        private MessageClient messageClientToServer = null;
        private MessageClient messageClientToOperator = null;

        public event MessageReceivedCallback MessageReceived;

        public User(string name, string email, string id)
        {
            this.Email = email;
            this.Name = name;
            this.ID = id;
            this.messageClientToOperator = new MessageClient();
            this.messageClientToServer = new MessageClient();
            messageClientToServer.MessageReceived += MessageClientToServer_MessageReceived;
            messageClientToOperator.MessageReceived += MessageClientToOperator_MessageReceived;
        }

        private void MessageClientToOperator_MessageReceived(Message message, IPEndPoint endPoint)
        {
            MessageReceived?.Invoke(message);
        }

        public void ConnectToOperator(IPEndPoint ipEndPoint)
        {
            messageClientToServer.Connect(ipEndPoint);
            var p2pRequest = new P2PRequest
            {
                UserEmail = this.Email,
                UserID = this.ID,
                UserName = this.Name
            };

            messageClientToServer.SendMessage(p2pRequest);
        }

        private void MessageClientToServer_MessageReceived(Message message, IPEndPoint endPoint)
        {
            if (message is P2PResponse)
            {
                var response = message as P2PResponse;
                var operatorEndPoint = response.ListeningEndPoint;
                messageClientToOperator.Connect(operatorEndPoint.ToIPEndPoint());
            }
        }

        public void SendMessage(Message message)
        {
            messageClientToOperator.SendMessage(message);
        }

    }
}
