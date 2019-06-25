using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MessageTransfer;
using MessageTransfer.Messages;
using UserImp;


namespace ClientConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            User user = new User("Hosein", "ssdwad", "a123");
            user.MessageReceived += User_MessageReceived;


            var serverEndPoint = new EndPoint
            {
                IP = "127.0.0.1",
                Port = 1111
            };

            user.ConnectToOperator(serverEndPoint);

            while(true)
            {
                var str = Console.ReadLine();
                if (str == "File")
                {
                    var fileMessage = new FileMessage
                    {
                        FileName = "google.mp3",
                        ID = 123,
                        Size = 852147853
                    };
                    user.SendMessage(fileMessage);
                    continue;
                }
                var message = new TextMessage
                {
                    DateTime = DateTime.Now,
                    Direction = TextDirection.LeftToRight,
                    Text = str
                };

                user.SendMessage(message);
            }
            
        }

        private static void User_MessageReceived(Message message)
        {
            if (message is TextMessage)
            {
                var str = (message as TextMessage).Text;
                Console.WriteLine(str);
            }
            else if (message is FileMessage)
            {
                var fileMessage = message as FileMessage;
                Console.WriteLine("File message received: File ID: {0}, File name: {1}, File size: {2}", 
                    fileMessage.ID, fileMessage.FileName, fileMessage.Size);
            }
        }
    }
}
