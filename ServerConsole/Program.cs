using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServerImp;


namespace ServerConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            Server server = new Server(1111);
            server.Start();
        }
    }
}
