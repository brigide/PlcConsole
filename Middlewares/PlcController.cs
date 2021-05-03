using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Linq;

namespace PlcConsole.Middlewares
{
    public class PlcController
    {
        public Dictionary<String, String> Telegrams { get; set; }

        public PlcController()
        {
            Telegrams = new Dictionary<string, string>();
            Telegrams.Add("PING", "MFS1;F001;XXXX;PING;...............................................................................\r");
            Telegrams.Add("PONG", "F001;MFS1;XXXX;PONG;...............................................................................\r");
            Telegrams.Add("LREP", "F001;MFS1;XXXX;LREP;YYYY;EEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEE,#DLLLLWWWWAAAA;R;SSSSS;MMMM;WPPPPP;ACK\r");
            Telegrams.Add("DLST", "MFS1;F001;XXXX;DLST;YYYY;EEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEE;DDDD;YYYY;TTTTTTTT;.............\r");
        }

        public async Task<String> SendMessageAndWaitResponseAsync(NetworkStream stream, String message)
        {
            byte[] byteArray = System.Text.Encoding.Default.GetBytes(message);
            await stream.WriteAsync(byteArray, 0, byteArray.Length);

            byte[] data = new byte[256];
            String response = String.Empty;

            Int32 bytes = await stream.ReadAsync(data, 0, data.Length);
            response = System.Text.Encoding.ASCII.GetString(data, 0 , bytes);

            return response;
        }

        public async Task<String> WaitMessageAsync(NetworkStream stream, byte[] data)
        {
            Int32 bytes = await stream.ReadAsync(data, 0, data.Length);
            String response = System.Text.Encoding.ASCII.GetString(data, 0 , bytes);

            response = HandleRecievedMessage(response);

            return response;
        }

        public String BuildMessage(String key, String cyclicCounter, String coCounter = null)
        {
            String response = String.Empty;
            if(key == "PING")
            {
                response = Telegrams[key].Replace("XXXX", cyclicCounter);
            }
            return response;
        }

        public String HandleRecievedMessage(String message)
        {
            String response = String.Empty;
            String key = message.Substring(15, 4);

            if(key == "PONG")
            {
                response = key;
            }
            else if(key ==  "LREP")
            {
                return response;
            }
            else if(key == "EXIT")
            {
                response = key;
            }

            return response;
        }
    }
}