using Microsoft.VisualStudio.TestTools.UnitTesting;
using FileTransfer;
using System.IO;
using System.Net;

namespace UnitTest
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            FileSender sender = new FileSender(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 1111));
            sender.LocalEndPoint = new IPEndPoint(IPAddress.Any, 1216);

            sender.SendFile(@"E:\Downloads\Music\سلام.mp3");
        }

        
    }


}
