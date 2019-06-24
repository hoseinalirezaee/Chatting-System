using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using MessageTransfer;
using MessageTransfer.Messages;


namespace Server
{
    class Program
    {
        public static void rcv(Message message, IPEndPoint endpoit)
        {
            Console.WriteLine("{0}: {1}", endpoit.ToString(), message.ToString());
        }

        static void Main(string[] args)
        {
            MessageServer server = new MessageServer(1111);

            server.MessageReceived += rcv;

            server.Start();

            while (true)
            {
                var str = Console.ReadLine();
                Message message = new TextMessage
                {
                    Text = str,
                    DateTime = DateTime.Now,
                    Direction = TextDirection.LeftToRight
                };

                server.SendMessage(message);
            }
        }
    }
}
