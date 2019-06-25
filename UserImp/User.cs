using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using MessageTransfer;
using MessageTransfer.Messages;
using FileTransfer;
using System.Net.Sockets;

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
        private Dictionary<int, FileInfo> filesList;
        private int lastFileIndex = 0;

        public event MessageReceivedCallback MessageReceived;

        public int GetFileID(FileInfo file)
        {
            foreach(var item in filesList)
            {
                if (item.Value == file)
                {
                    return item.Key;
                }
            }
            return -1;
        }

        public FileInfo GetFile(int id)
        {
            FileInfo file;
            if (filesList.TryGetValue(id, out file))
            {
                return file;
            }
            return null;
        }

        public User(string name, string email, string id)
        {
            this.Email = email;
            this.Name = name;
            this.ID = id;
            this.messageClientToOperator = new MessageClient();
            this.messageClientToServer = new MessageClient();
            this.filesList = new Dictionary<int, FileInfo>();
            messageClientToServer.MessageReceived += MessageClientToServer_MessageReceived;
            messageClientToOperator.MessageReceived += MessageClientToOperator_MessageReceived;
        }

        private void MessageClientToOperator_MessageReceived(Message message, IPEndPoint endPoint)
        {
            if (message is FileMessage)
            {
                var fileMessage = message as FileMessage;
                filesList.Add(fileMessage.ID, null);
                if (fileMessage.ID > lastFileIndex)
                {
                    lastFileIndex = fileMessage.ID;
                }
            }
            if (message is FileRequest)
            {
                var fileRequest = message as FileRequest;
                var file = filesList[fileRequest.ID];
                FileSender fileSender = new FileSender(fileRequest.RecieverEndPoint.ToIPEndPoint());
                fileSender.SendFile(file);
            }
            else
            {
                MessageReceived?.Invoke(message);
            }
        }

        public void ConnectToOperator(MessageTransfer.EndPoint serverEndPoint)
        {
            messageClientToServer.Connect(serverEndPoint.ToIPEndPoint());
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
                messageClientToServer.CloseConnection();
                var response = message as P2PResponse;
                var operatorEndPoint = response.ListeningEndPoint;
                messageClientToOperator.Connect(operatorEndPoint.ToIPEndPoint());
            }
        }

        public void SendMessage(Message message)
        {
            messageClientToOperator.SendMessage(message);
        }

        public void SendFile(FileInfo file)
        {
            var fileName = file.Name;
            var size = file.Length;
            var id = ++lastFileIndex;
            filesList.Add(id, file);

            FileMessage fileMessage = new FileMessage
            {
                FileName = fileName,
                ID = id,
                Size = size
            };

            SendMessage(fileMessage);
        }

        public void RecevieFile(int id, string path)
        {
            ThreadStart threadStart = new ThreadStart(() => { ReceiveFileThread(id, path); });
            Thread thread = new Thread(threadStart);
            thread.Start();
            thread.Join();
        }

        private void ReceiveFileThread(int id, string path)
        {
            var port = GetRandomPort();

            Message fileRequest = new FileRequest
            {
                ID = id,
                RecieverEndPoint = new MessageTransfer.EndPoint { IP = "127.0.0.1", Port = port }
            };

            SendMessage(fileRequest);

            FileReciever fileReciever = new FileReciever(port);

            fileReciever.RecieveFile(messageClientToOperator.RemoteEndPoint, new DirectoryInfo(path));

            filesList[id] = fileReciever.File;

        }

        public int GetRandomPort()
        {
            TcpListener listener = new TcpListener(IPAddress.Loopback, 0);
            listener.Start();
            var port = (listener.LocalEndpoint as IPEndPoint).Port;
            listener.Stop();
            return port;
        }
    }
}
