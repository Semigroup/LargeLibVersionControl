using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LLVC
{
   public class Protocol : ICloneable
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

        public HashValue Concat(HashFunction HashFunction, HashValue h1, HashValue h2)
        {
            byte[] concat = h1 * h2;
            return HashFunction.ComputeHash(concat);
        }

        public Commit CheckHashes(HashFunction HashFunction)
        {
            HashValue currentHash = InitialHash;
            foreach (var c in Commits)
            {
                if (Concat(HashFunction, currentHash, c.Diff.ComputeHash(HashFunction)) != c.Hash)
                    return c;
                currentHash = c.Hash;
            }
            return null;
        }

        public object Clone()
            => new Protocol()
            {
                InitialHash = InitialHash,
                LibraryName = LibraryName,
                Commits = Commits.Select(x => x.Clone() as Commit).ToList()
            };
    }
}
