using Microsoft.VisualStudio.TestTools.UnitTesting;
using MessageTransfer;
using MessageTransfer.Messages;
using System.Net;
using System;

namespace ServerTest
{
    [TestClass]
    public class Server
    {
        [TestMethod]
        public void TestMethod1()
        {
            MessageServer server = new MessageServer(1111);

            server.MessageReceived += rcv;

            server.Start();

            while(true)
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

        public void rcv(Message message, IPEndPoint endpoit)
        {
            Console.WriteLine("{0}: {1}", endpoit.ToString(), message.ToString());
        }
    }
}
