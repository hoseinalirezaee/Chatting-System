using Microsoft.VisualStudio.TestTools.UnitTesting;
using MessageTransfer.Messages;
using MessageTransfer;
using System.Net;
using System;
using System.Xml.Serialization;
using System.Xml;
using System.Text;
using System.IO;

namespace ClientTest
{
    [TestClass]
    public class Client
    {
        [TestMethod]
        public void TestMethod1()
        {
            XmlSerializer serializer = new XmlSerializer(typeof(Message));

            Message message = new TextMessage
            {
                Text = "Hello",
                DateTime = DateTime.Now,
                Direction = TextDirection.LeftToRight
            };

            StringBuilder stringBuilder = new StringBuilder();
            XmlWriter writer = XmlWriter.Create(stringBuilder,
                new XmlWriterSettings { NewLineHandling = NewLineHandling.None });

            serializer.Serialize(writer, message);

            writer.Close();

            var s = stringBuilder.ToString();

            StringReader reader = new StringReader(s);

            var m = serializer.Deserialize(reader) as TextMessage;
            

        }

      
    }
}
