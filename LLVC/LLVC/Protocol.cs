using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;

namespace LLVC
{
   public class Protocol
    {
        public HashValue InitialHash { get; set; }
        public string LibraryName { get; set; }
        public List<Commit> Commits { get; set; }

        private Protocol()
        {

        }

        public Protocol(string LibraryName, HashValue InitialHash)
        {
            this.LibraryName = LibraryName;
            this.InitialHash = InitialHash;
            this.Commits = new List<Commit>();
        }

        public int CheckNumbering()
        {
            int i = 0;
            foreach (var c in Commits)
                if (c.Number != i)
                    return i;
                else
                    i++;
            return -1;
        }

        public HashValue Concat(HashAlgorithm HashAlgorithm, HashValue h1, HashValue h2)
        {
            byte[] concat = h1 * h2;
            return new HashValue(HashAlgorithm.ComputeHash(concat));
        }

        public Commit CheckHashes(HashAlgorithm HashAlgorithm)
        {
            HashValue currentHash = InitialHash;
            foreach (var c in Commits)
            {
                if (Concat(HashAlgorithm, currentHash, c.Diff.ComputeHash(HashAlgorithm)) != c.Hash)
                    return c;
                currentHash = c.Hash;
            }
            return null;
        }
    }
}
