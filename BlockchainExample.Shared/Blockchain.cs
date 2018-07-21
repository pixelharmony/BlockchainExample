using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BlockchainExample.Shared
{
    public class Blockchain
    {
        public List<Block> Chain { get; set; }
        public int Difficulty { get; set; }
        public List<Transaction> PendingTransactions = new List<Transaction>();
        public int Reward { get; set; }

        public Blockchain()
        {
            Chain = new List<Block>();
            Difficulty = 2;
            Reward = 1;
            CreateInitialBlock();
        }

        public Block GetLatestBlock()
        {
            return Chain.Last();
        }

        public void CreateInitialBlock()
        {
            var block = new Block(DateTime.Now, null, null);
            block.Mine(Difficulty);
            Chain.Add(block);
        }

        public void AddBlock(Block block)
        {
            var latestBlock = GetLatestBlock();
            block.Index = latestBlock.Index + 1;
            block.PreviousHash = latestBlock.Hash;
            block.Transactions.AddRange(PendingTransactions);
            PendingTransactions = new List<Transaction>();
            block.Mine(Difficulty);
            Chain.Add(block);
        }

        public void CreateTransaction(Transaction transaction)
        {
            PendingTransactions.Add(transaction);
        }

        public void ProcessPendingTransactions(string minerAddress)
        {
            var block = new Block(DateTime.Now, GetLatestBlock().Hash, PendingTransactions);
            AddBlock(block);

            PendingTransactions = new List<Transaction>();
            CreateTransaction(new Transaction(null, minerAddress, Reward));
        }

        public bool IsValid()
        {
            return IsValid(Chain);
        }

        private bool IsValid(List<Block> chain)
        {
            if (chain.Count <= 1)
                return true;

            for (int i = 1; i < chain.Count; i++)
            {
                var currentBlock = chain[i];
                var previousBlock = chain[i - 1];

                if (currentBlock.Hash != currentBlock.CalculateHash() || currentBlock.PreviousHash != previousBlock.Hash)
                {
                    return false;
                }
            }

            return true;
        }

        public bool UpdateBlockchain(List<Block> chain)
        {
            if (chain != null && chain.Count > Chain.Count && IsValid(chain))
            {
                Chain = chain;
                return true;
            }

            return false;
        }
    }
}
