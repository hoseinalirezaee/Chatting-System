using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OperatorImp;
using MessageTransfer;
using MessageTransfer.Messages;
namespace OperatorConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            Operator @operator = new Operator(1);
            @operator.MessageReceived += operator_MessageReceived;

            var serverEndPoint = new EndPoint
            {
                IP = "127.0.0.1",
                Port = 1111
            };

            @operator.ConnectToUser(serverEndPoint);

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
                    @operator.SendMessage(fileMessage);
                    continue;
                }

                var message = new TextMessage
                {
                    DateTime = DateTime.Now,
                    Direction = TextDirection.LeftToRight,
                    Text = str
                };

                @operator.SendMessage(message);
            }

            
        }

        private static void operator_MessageReceived(MessageTransfer.Messages.Message message)
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
