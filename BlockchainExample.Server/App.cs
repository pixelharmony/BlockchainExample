using BlockchainExample.Shared;
using Consul;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BlockchainExample.Server
{
    enum MenuOptions
    {
        MineBlock = 1,
        DisplayBlockhain = 2,
        Quit = 3
    }

    class App
    {
        private TcpServer _server;
        private Blockchain _blockchain;
        private readonly IConsulClient _consulClient;
        private readonly string _nodeId;

        public App(IConsulClient consulClient, TcpServer server)
        {
            _blockchain = new Blockchain();
            _consulClient = consulClient;
            _server = server;
            _server.OnMessageReceived = OnMessageReceieved;
            _nodeId = $"blockchain-node-{_server.Port}";
        }

        public void Start()
        {

            StartNode();
            Console.WriteLine($"Started node on port {_server.Port}");

            var appRunning = true;

            while (appRunning)
            {
                Console.WriteLine("Select an option");
                Console.WriteLine("1. Mine Block");
                Console.WriteLine("2. Display Local Blockchain");
                Console.WriteLine("3. Quit");

                int selectedOption;
                while (!int.TryParse(Console.ReadLine(), out selectedOption) && new[] { 1, 2, 3 }.Contains(selectedOption))
                    Console.WriteLine("Please input a valid option");

                Console.Clear();

                switch ((MenuOptions)selectedOption)
                {
                    case MenuOptions.MineBlock:
                        MineBlock();
                        break;
                    case MenuOptions.DisplayBlockhain:
                        DisplayLocalBlockchain();
                        break;
                    case MenuOptions.Quit:
                        appRunning = false;
                        break;
                }

                Console.WriteLine(string.Empty);
                Console.WriteLine("====================================");
                Console.WriteLine(string.Empty);
            }

            StopNode();
        }

        private void StartNode()
        {
            _server.StartServer();

            var registration = new AgentServiceRegistration()
            {
                ID = _nodeId,
                Name = "Blockchain Node",
                Address = _server.IpAddress,
                Port = _server.Port,
                Tags = new[] { "blockchain-node" }
            };

            _consulClient.Agent.ServiceDeregister(_nodeId);
            _consulClient.Agent.ServiceRegister(registration);
        }

        private void StopNode()
        {
            _consulClient.Agent.ServiceDeregister(_nodeId);
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
            var availableServices = _consulClient.Agent.Services().Result.Response
                .Where(x => x.Value.Tags.Contains("blockchain-node") && x.Key != _nodeId).ToList();

            foreach (var service in availableServices)
            {
                var json = JsonConvert.SerializeObject(_blockchain.Chain);
                using (var client = new TcpClient(service.Value.Address, service.Value.Port))
                {
                    client.Connect();
                    client.Send(json);
                }

                Console.WriteLine($"Sent blockchain update to remote node: {service.Key}");
            }
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
            var result = _blockchain.UpdateBlockchain(chain);

            if (result)
            {
                Console.WriteLine("Update received from remote node, new blockchain accepted");
            } else
            {
                Console.WriteLine("Update received from remote node, new blockchain rejected");
            }
        }
    }
}
