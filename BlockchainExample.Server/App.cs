using BlockchainExample.Shared;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BlockchainExample.Server
{
    enum MenuOptions
    {
        StartServer = 1,
        StopServer = 2,
        MineBlock = 3,
        DisplayBlockhain = 4,
        Quit = 5
    }

    class App
    {
        private TcpServer _server;
        private Blockchain _blockchain;

        public App()
        {
            _blockchain = new Blockchain();
        }

        private List<MenuOptions> GetValidMenuOptions()
        {
            var options = new List<MenuOptions>()
            {
                MenuOptions.DisplayBlockhain
            };

            if (_server == null || !_server.IsRunning)
            {
                options.Add(MenuOptions.StartServer);
            }
            else
            {
                options.Add(MenuOptions.StopServer);
                options.Add(MenuOptions.MineBlock);
            }

            return options;
        }

        public bool ExecuteMenuOption(MenuOptions option)
        {
            if (!GetValidMenuOptions().Contains(option))
                return false;

            switch (option)
            {
                case MenuOptions.StartServer:
                    StartServer();
                    break;
                case MenuOptions.StopServer:
                    StopServer();
                    break;
                case MenuOptions.MineBlock:
                    MineBlock();
                    break;
                case MenuOptions.DisplayBlockhain:
                    DisplayLocalBlockchain();
                    break;
            }

            return true;
        }

        private void StartServer()
        {
            Console.WriteLine("Enter Port to start server on, (use 6001, 6002 or 6003)");
            int serverPort;
            while (!int.TryParse(Console.ReadLine(), out serverPort))
            {
                Console.WriteLine("Specified port is in valid, try again...");
            }

            _server = TcpServer.StartServer(serverPort, OnMessageReceieved);
            Console.WriteLine($"Started node on port {serverPort}");
        }

        private void StopServer()
        {
            _server.Dispose();
            Console.WriteLine("Server stopped");
        }

        private void MineBlock()
        {
            _blockchain.AddBlock(new Block(DateTime.Now, null, null));
            Console.WriteLine("Mined Block successfully");

            UpdateNetwork();
        }

        private void UpdateNetwork()
        {
            var ports = new[] { 6001, 6002, 6003 };

            var remoteServers = ports.Where(x => x != _server.Port).ToArray();

            foreach (var remote in remoteServers)
            {
                var json = JsonConvert.SerializeObject(_blockchain.Chain);
                using (var client = new TcpClient(remote))
                {
                    client.Connect();
                    client.Send(json);
                }

                Console.WriteLine($"Updated remote node, 127.0.0.1:{remote}");
            }

            Console.WriteLine("Updated network of Blockchain changes");
        }

        private void DisplayLocalBlockchain()
        {
            Console.WriteLine(JsonConvert.SerializeObject(_blockchain.Chain, Formatting.Indented));

            if (_blockchain.IsValid())
            {
                Console.WriteLine("Blockchain is VALID");
            }
            else
            {
                Console.WriteLine("Blockchain is NOT VALID");
            }
        }

        private void OnMessageReceieved(string message)
        {
            var chain = JsonConvert.DeserializeObject<List<Block>>(message);
            _blockchain.UpdateBlockchain(chain);

            Console.WriteLine("Message Received, attempting to update blockchain");
        }
    }
}
