using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
//using System.Net;
//using System.Net.Sockets;
using System.Text;
using System.Threading;
using FileTransfer;
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
        private Dictionary<int, FileInfo> filesList;
        private int lastFileIndex = 0;

        public event MessageReceivedCallback MessageReceived;

        public FileInfo GetFile(int id)
        {
            FileInfo file;
            if (filesList.TryGetValue(id, out file))
            {
                return file;
            }
            return null;
        }

        public Operator(int id)
        {
            ID = id;
            messageClient = new MessageClient();
            filesList = new Dictionary<int, FileInfo>();
            
        }

        public int GetFileID(FileInfo file)
        {
            foreach (var item in filesList)
            {
                if (item.Value == file)
                {
                    return item.Key;
                }
            }
            return -1;
        }

        public void ConnectToUser(MessageTransfer.EndPoint serverEndPoint)
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

        public void SendMessage(Message message)
        {
            messageServer.SendMessage(message);
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

            fileReciever.RecieveFile(messageClient.RemoteEndPoint, new DirectoryInfo(path));

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
