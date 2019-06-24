using MessageTransfer.Messages;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace MessageTransfer
{
    public delegate void MessageReceivedCallback(Message message, IPEndPoint endPoint);

    public class MessageServer
    {
        private ConcurrentDictionary<IPEndPoint, ClientInfo> clients = null;
        private Thread listenerThread = null;
        private Thread receiverThread = null;
        private TcpListener listenerSocket = null;

        public MessageTransfer.EndPoint ListeningEndPoint
        {
            get
            {
                return EndPoint.Parse(listenerSocket.Server.LocalEndPoint as IPEndPoint);
            }
        }

        public event MessageReceivedCallback MessageReceived;

        public MessageServer() : this(FreeTcpPort())
        {
        }

        public MessageServer(int port)
        {
            clients = new ConcurrentDictionary<IPEndPoint, ClientInfo>();

            IPEndPoint localEP = new IPEndPoint(IPAddress.Any, port);
            listenerSocket = new TcpListener(localEP);
        }

        private void ListenerMethod()
        {
            listenerSocket.Start();

            while(true)
            {
                var client = listenerSocket.AcceptTcpClient();
                ClientInfo clientInfo = new ClientInfo
                {
                    ClientSocket = client,
                    MessageSender = new MessageSender(client),
                    MessageReceiver = new MessageReceiver(client)
                };
                clients.GetOrAdd(client.Client.RemoteEndPoint as IPEndPoint, clientInfo);
            }
        }

        private void ReceiverMethod()
        {
            List<Socket> list = new List<Socket>();

            while(true)
            {
                while (clients.IsEmpty) ;

                list.Clear();

                foreach (var item in clients)
                {
                    list.Add(item.Value.ClientSocket.Client);
                }

                Socket.Select(list, null, null, -1);

                foreach (var item in list)
                {
                    var endPoint = item.RemoteEndPoint as IPEndPoint;
                    var receiver = clients[endPoint].MessageReceiver;
                    var message = receiver.ReceiveMessage();
                    MessageReceived?.Invoke(message, endPoint);
                }
            }
        }

        public void SendMessage(Message message, IPEndPoint endPoint)
        {
            var sender = clients[endPoint].MessageSender;
            sender.SendMessage(message);
        }

        public void SendMessage(Message message)
        {
            foreach(var item in clients)
            {
                SendMessage(message, item.Key);
            }
        }

        public void Start()
        {
            ThreadStart setting = new ThreadStart(ListenerMethod);
            listenerThread = new Thread(setting);
            listenerThread.Start();

            StartReceiving();
        }

        private void StartReceiving()
        {
            ThreadStart setting = new ThreadStart(ReceiverMethod);
            receiverThread = new Thread(setting);
            receiverThread.Start();
        }

        public void CloseConnection(IPEndPoint endPoint)
        {
            ClientInfo client;
            if (clients.TryGetValue(endPoint, out client))
            {
                client.ClientSocket.Close();
                clients.TryRemove(endPoint, out client);
            }
        }

        static int FreeTcpPort()
        {
            TcpListener l = new TcpListener(IPAddress.Loopback, 0);
            l.Start();
            int port = ((IPEndPoint)l.LocalEndpoint).Port;
            l.Stop();
            return port;
        }
    }
}
