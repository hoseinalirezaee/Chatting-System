using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace MessageTransfer
{
    public class EndPoint
    {
        public int Port
        {
            get;
            set;
        }
        public string IP { get; set; }

        public IPEndPoint ToIPEndPoint()
        {
            IPAddress ip = IPAddress.Parse(IP);
            IPEndPoint iPEndPoint = new IPEndPoint(ip, Port);
            return iPEndPoint;
        }

        public override bool Equals(object obj)
        {
            return obj is EndPoint point &&
                   Port == point.Port &&
                   IP == point.IP;
        }

        public override int GetHashCode()
        {
            var hashCode = 90932370;
            hashCode = hashCode * -1521134295 + Port.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(IP);
            return hashCode;
        }

        public static bool operator ==(EndPoint left, EndPoint right)
        {
            return EqualityComparer<EndPoint>.Default.Equals(left, right);
        }

        public static bool operator !=(EndPoint left, EndPoint right)
        {
            return !(left == right);
        }

        public static EndPoint Parse(IPEndPoint ipEndPoint)
        {
            var ip = ipEndPoint.Address.ToString();
            var port = ipEndPoint.Port;

            EndPoint endPoint = new EndPoint
            {
                IP = ip,
                Port = port
            };
            return endPoint;
        }
    }
}
