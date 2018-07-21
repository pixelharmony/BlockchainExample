using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace BlockchainExample.Server
{
    class TcpServer : IDisposable
    {
        public bool IsRunning { get; private set; }
        public int Port { get; private set; }
        private TcpListener Listener { get; set; }
        private Action<string> OnMessageReceived { get; set; }

        private TcpServer(IPAddress address, int port, Action<string> onMessageReceived) {
            OnMessageReceived = onMessageReceived;
            Listener = new TcpListener(address, port);
            Listener.Start();
            WaitForClients();
            IsRunning = true;
            Port = port;
        }

        public static TcpServer StartServer(int port, Action<string> onMessageReceived)
        {
            var address = IPAddress.Parse("127.0.0.1");
            var server = new TcpServer(address, port, onMessageReceived);

            return server;
        }

        public void StopServer()
        {
            Listener.Server.Close();
            Listener.Stop();
            IsRunning = false;
        }

        private void WaitForClients()
        {
            Listener.BeginAcceptTcpClient(new AsyncCallback(OnClientConnected), null);
        }

        private void OnClientConnected(IAsyncResult asyncResult)
        {
            if (!Listener.Server.IsBound)
                return;

            using (var client = Listener.EndAcceptTcpClient(asyncResult))
            {
                if (client != null)
                {
                    HandleClientRequest(client);
                }
            }
            WaitForClients();
        }

        private void HandleClientRequest(System.Net.Sockets.TcpClient client)
        {
            var buffer = new byte[1024];
            var numberOfBytesRead = 0;
            using (var stream = client.GetStream())
            {
                var message = new StringBuilder();
                do
                {
                    numberOfBytesRead = stream.Read(buffer, 0, buffer.Length);
                    message.AppendFormat("{0}", Encoding.ASCII.GetString(buffer, 0, numberOfBytesRead));
                } while (stream.DataAvailable);

                OnMessageReceived(message.ToString());
            }
        }

        public void Dispose()
        {
            StopServer();
        }
    }
}
