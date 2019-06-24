using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using MessageTransfer;
using MessageTransfer.Messages;
using MessageTransfer.Exceptions;
namespace Client
{
    class Program
    {
        private static void Client_MessageReceived(Message message, IPEndPoint endPoint)
        {
            Console.WriteLine("{0}: {1}", endPoint.ToString(), message.ToString());
        }

        static void Main(string[] args)
        {
            IPEndPoint iPEndPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 1212);

            var endPoint = MessageTransfer.EndPoint.Parse(iPEndPoint);

            

        }
    }
}
