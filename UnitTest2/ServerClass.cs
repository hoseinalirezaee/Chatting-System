using FileTransfer;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Net;

namespace UnitTest2
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod2()
        {
            FileReciever reciever = new FileReciever(1111);

            reciever.RecieveFile(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 1212), @"C:\Users\Hosein\Desktop");
        }
    }
}
