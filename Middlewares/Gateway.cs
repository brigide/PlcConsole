using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Linq;

namespace PlcConsole.Middlewares
{
    /// <summary>
    /// A connection to an PLC
    /// </summary>
    public class Gateway : IDisposable
    {
        private NetworkStream stream;

        private int broadcast;

        /// <summary>
        /// Controller connection instance
        /// </summary>
        private TcpClient client;

        /// <summary>
        /// Gateway IP address
        /// </summary>
        public string Ip { get; set; }

        /// <summary>
        /// Port used for connection
        /// </summary>
        public int Port { get; set; }

        private Gateway()
        {
            client = new TcpClient();
        }

        /// <summary>
        /// Creates a connection to an PLC Gateway
        /// </summary>
        /// <param name="ip">Gateway IP</param>
        /// <param name="port">Gateway Port</param>
        /// <param name="broadcast">Tag address related to broadcast operation</param>
        /// <returns></returns>
        async public static Task<Gateway> FromAddress(string ip, int port, int broadcast = 252)
        {
            var controller = new Gateway();
            await controller.ConnectAsync(ip, port);
            controller.broadcast = broadcast;
            controller.stream = controller.client.GetStream();
            await controller.SendMessage("Connected");
            return controller;
        }

        /// <summary>
        /// Creates and estabilishes a TcpClient connection
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="port"></param>
        /// <returns></returns>
        async public Task ConnectAsync(string ip, int port)
        {
            Ip = ip;
            Port = port;
            await client.ConnectAsync(ip, port);
        }

        async public Task<String> SendMessage(String message)
        {
            byte[] byteArray = System.Text.Encoding.Default.GetBytes(message);

            await stream.WriteAsync(byteArray, 0, byteArray.Length);

            byte[] data = new byte[256];
            String response = String.Empty;

            Int32 bytes = await stream.ReadAsync(data, 0, data.Length);
            response = System.Text.Encoding.ASCII.GetString(data, 0 , bytes);

            return message;
        }





























        #region IDisposable Support
        private bool disposedValue = false; // Para detectar chamadas redundantes

        // Retirar daqui depois
        public async Task<byte[]> WaitResponse()
        {
            var dummyData = new byte[15];
            await stream.ReadAsync(dummyData, 0, 15);
            return dummyData;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    client.Dispose();
                }

                // TODO: liberar recursos não gerenciados (objetos não gerenciados) e substituir um finalizador abaixo.
                // TODO: definir campos grandes como nulos.

                disposedValue = true;
            }
        }

        // TODO: substituir um finalizador somente se Dispose(bool disposing) acima tiver o código para liberar recursos não gerenciados.
        // ~Gateway() {
        //   // Não altere este código. Coloque o código de limpeza em Dispose(bool disposing) acima.
        //   Dispose(false);
        // }

        // Código adicionado para implementar corretamente o padrão descartável.
        public void Dispose()
        {
            // Não altere este código. Coloque o código de limpeza em Dispose(bool disposing) acima.
            Dispose(true);
            // TODO: remover marca de comentário da linha a seguir se o finalizador for substituído acima.
            // GC.SuppressFinalize(this);
        }
        #endregion
    }
}