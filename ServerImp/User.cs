using System;
using System.Collections.Generic;
using System.Text;
using MessageTransfer;

namespace ServerImp
{
    public class User
    {
        public string UserName { get; set; }
        public string UserEmail { get; set; }
        public string UserID { get; set; }
        public EndPoint EndPoint { get; set; }
        public int State { get; set; }
    }
}
