using System;
using System.Net;
using System.Text;

namespace BlockchainExample.Server
{
    class TcpClient : IDisposable
    {
        private readonly string _ipAddress;
        private readonly int _port;
        private readonly System.Net.Sockets.TcpClient _client;

        public TcpClient(string ipAddress, int port)
        {
            _ipAddress = ipAddress;
            _port = port;
            _client = new System.Net.Sockets.TcpClient();
        }

        public void Connect()
        {
            var ipAddress = IPAddress.Parse(_ipAddress);
            _client.Connect(ipAddress, _port);
        }

        public void Send(string message)
        {
            var data = Encoding.ASCII.GetBytes(message);
            using (var stream = _client.GetStream())
            {
                stream.Write(data, 0, data.Length);
            }
        }

        public void Close()
        {
            _client.Close();
        }

        public void Dispose()
        {
            Close();
            _client.Dispose();
        }
    }
}
