using System;
using System.Collections.Generic;
using System.Text;

namespace MessageTransfer.Exceptions
{
    public class RemoteClientShutDown : Exception
    {
        public override string Message => "Remote client disconnected.";
    }
}
