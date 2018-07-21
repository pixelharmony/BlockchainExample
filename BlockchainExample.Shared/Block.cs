using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace BlockchainExample.Shared
{
    public class Block
    {
        public int Index { get; set; }
        public DateTime Timestamp { get; set; }
        public string PreviousHash { get; set; }
        public string Hash { get; set; }
        public List<Transaction> Transactions { get; set; }
        public int Nonce { get; set; }

        public Block(DateTime timeStamp, string previousHash, List<Transaction> pendingTransactions)
        {
            Index = 0;
            Timestamp = timeStamp;
            PreviousHash = previousHash;
            Transactions = pendingTransactions ?? new List<Transaction>();
            Hash = CalculateHash();
            Nonce = 0;
        }

        public string CalculateHash()
        {
            using(var algorithm = SHA256.Create())
            {
                var inputBytes = Encoding.ASCII.GetBytes($"{Timestamp}-{PreviousHash ?? string.Empty}-{JsonConvert.SerializeObject(Transactions)}-{Nonce}");
                var outputBytes = algorithm.ComputeHash(inputBytes);

                return Convert.ToBase64String(outputBytes);
            }
        }

        public void Mine(int difficulty)
        {
            var leadingZeros = new string('0', difficulty);
            while (Hash == null || Hash.Substring(0, difficulty) != leadingZeros)
            {
                Nonce++;
                Hash = CalculateHash();
            }
        }
    }
}
