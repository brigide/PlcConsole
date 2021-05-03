using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using PlcConsole.Middlewares;

namespace PlcConsole
{
    class Program
    {
        static async Task Main(string[] args)
        {
            PlcController controller = new PlcController();

            Plc plc1 = new Plc("192.168.15.6", 2000);

            await plc1.Init();
            
        }
    }
}
