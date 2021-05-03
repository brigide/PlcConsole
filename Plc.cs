using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using PlcConsole.Middlewares;


namespace PlcConsole
{
    public class Plc
    {
        public String Ip { get; set; }
        public Int32 Port { get; set; }
        public TcpClient client { get; set; }
        public NetworkStream stream { get; set; }
        private PlcController controller;
        
        private byte[] data { get; set; }
        private String response { get; set; }
        private String message { get; set; }
        public Plc(String ip, Int32 port)
        {
            Ip = ip;
            Port = port;

            data = new byte[256];

            response = String.Empty;

            message = String.Empty;

            controller = new PlcController();
        }

        public async Task Init()
        {
            String cyclicCounter = String.Empty;
            String coCounter = String.Empty;
            String readValue = String.Empty;
            
            await ConnectAsync(Ip, Port);

            while(true)
            {
                response = await controller.WaitMessageAsync(stream, data);
                Console.WriteLine(response);

                if(response.Substring(15, 4) == "LREP")
                {
                    message = controller.BuildMessage("DLST", response.Substring(11, 4), response.Substring(20, 4));

                    response = await controller.SendMessageAndWaitResponseAsync(stream, message);

                    response = await controller.WaitMessageAsync(stream, data);
                    Console.WriteLine(response);
                }

                if(response == "EXIT")
                {
                    break;
                }
            }

            CloseConnection();
        }   

        private async Task ConnectAsync(String ip, Int32 Port)
        {
            client = new TcpClient();
            await client.ConnectAsync(Ip, Port);

            stream = client.GetStream();

            message = controller.BuildMessage("PING", "0001");

            Console.WriteLine("Connected!");
            Console.WriteLine("Sent message: " + message + "\n");

            String response = await controller.SendMessageAndWaitResponseAsync(stream, message);

            response = await controller.WaitMessageAsync(stream, data);

            Console.WriteLine("Recieved message: " + response + "\n");
        }

        private void CloseConnection()
        {
            stream.Close();
            client.Close();

            Console.WriteLine("\nConnection closed");
        }
    }
}