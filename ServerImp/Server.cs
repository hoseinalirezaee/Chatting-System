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

                Console.WriteLine("P2P request from {0}: Name: {1}, User ID: {2}, Email: {3}",
                    endPoint.ToString(), user.UserName, user.UserID, user.UserEmail);

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
                Console.WriteLine("Ready request from {0}, End Point: {1}",
                    @operator.OperatorID, @operator.EndPoint.ToIPEndPoint().ToString());
                messageServer.CloseConnection(endPoint);
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
                Console.WriteLine("P2P responde sent to user {0}, {1}",
                    user.UserName, user.EndPoint.ToIPEndPoint().ToString());
                messageServer.CloseConnection(userEndPoint.ToIPEndPoint());
            }
        }

    }
}
