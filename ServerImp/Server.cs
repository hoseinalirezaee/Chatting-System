using System;
using System.Collections.Generic;
using System.Text;
using MessageTransfer;
using MessageTransfer.Messages;
using System.Collections.Concurrent;
using System.Net;

namespace ServerImp
{
    public class Server
    {
        private MessageServer messageServer = null;
        private Queue<User> penddingUsers = null;
        private Queue<Operator> readyOperators = null;

        public Server(int port)
        {
            messageServer = new MessageServer(port);
            penddingUsers = new Queue<User>();
            readyOperators = new Queue<Operator>();
        }

        public void Start()
        {
            messageServer.MessageReceived += ServerFunction;
            messageServer.Start();
        }

        private void ServerFunction(Message message, IPEndPoint endPoint)
        {
            User user;
            Operator @operator;

            if (message is P2PRequest)
            {
                var request = message as P2PRequest;
                user = new User
                {
                    EndPoint = MessageTransfer.EndPoint.Parse(endPoint),
                    State = 0,
                    UserEmail = request.UserEmail,
                    UserID = request.UserID,
                    UserName = request.UserName
                };
                penddingUsers.Enqueue(user);
                SendConnectionInfoToUser();
            }
            else if (message is ReadyRequest)
            {
                var request = message as ReadyRequest;
                @operator = new Operator
                {
                    EndPoint = MessageTransfer.EndPoint.Parse(endPoint),
                    ListeningEndPoint = request.EndPoint,
                    OperatorID = request.OperatorID,
                    State = 0
                };
                readyOperators.Enqueue(@operator);
                SendConnectionInfoToUser();
            }
            else
            {
                messageServer.CloseConnection(endPoint);
            }
        }

        private void SendConnectionInfoToUser()
        {
            if (readyOperators.Count != 0 && penddingUsers.Count != 0)
            {
                var user = penddingUsers.Dequeue();
                var @operator = readyOperators.Dequeue();

                var operatorEndPoint = @operator.ListeningEndPoint;
                var userEndPoint = user.EndPoint;

                P2PResponse p2PResponse = new P2PResponse
                {
                    ListeningEndPoint = operatorEndPoint,
                    Status = Status.Ok
                };

                messageServer.SendMessage(p2PResponse, userEndPoint.ToIPEndPoint());
            }
        }

    }
}
