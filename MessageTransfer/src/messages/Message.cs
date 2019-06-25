using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace MessageTransfer.Messages
{
    [XmlInclude(typeof(TextMessage))]
    [XmlInclude(typeof(FileMessage))]
    [XmlInclude(typeof(FileRequest))]
    [XmlInclude(typeof(P2PRequest))]
    [XmlInclude(typeof(P2PResponse))]
    [XmlInclude(typeof(ReadyRequest))]
    [XmlInclude(typeof(ReadyResponse))]
    [XmlInclude(typeof(ShutdownRequest))]
    [XmlInclude(typeof(ShutdownResponse))]
    [XmlInclude(typeof(StartupRequest))]
    [XmlInclude(typeof(StartupResponse))]
    [XmlInclude(typeof(VerificationRequest))]
    [XmlInclude(typeof(VerificationResponse))]
    [XmlInclude(typeof(Introduction))]
    public abstract class Message
    {
        public abstract int Type
        {
            get;
        }
    }
}
