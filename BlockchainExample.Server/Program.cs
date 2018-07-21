using BlockchainExample.Shared;
using Newtonsoft.Json;
using System;

namespace BlockchainExample.Server
{
    class Program
    {
        private static Blockchain blockchain = new Blockchain();

        static void Main(string[] args)
        {
            Console.WriteLine("Example Distributed Blockchain");

            bool running = true;
            var app = new App();

            while (running)
            {
                // Read option input
                Console.WriteLine("Select an option");
                Console.WriteLine("1: Start Server");
                Console.WriteLine("2: Stop Server");
                Console.WriteLine("3: Mine Block");
                Console.WriteLine("4: Display Blockchain");
                Console.WriteLine("5: Quit");

                int selectedOption;
                while (!int.TryParse(Console.ReadLine(), out selectedOption))
                    Console.WriteLine("Select a valid option");

                if ((MenuOptions)selectedOption != MenuOptions.Quit)
                {
                    var result = app.ExecuteMenuOption((MenuOptions)selectedOption);

                    if (!result)
                        Console.WriteLine("Can't do that right now, pick another option");

                    Console.WriteLine(string.Empty);
                    Console.WriteLine("===========================================================");
                    Console.WriteLine(string.Empty);
                }
                else
                {
                    running = false;
                }
            }

        }
    }
}
