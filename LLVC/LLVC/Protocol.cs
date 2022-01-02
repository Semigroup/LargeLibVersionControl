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
        public HashValue InitialHash { get; private set; }
        public string LibraryName { get; private set; }
        public IList<Commit> Commits { get; private set; }
        public Index Index { get; private set; }

        public Protocol(string LibraryName, HashValue InitialHash)
        {
            this.LibraryName = LibraryName;
            this.InitialHash = InitialHash;
            this.Commits = new List<Commit>();
            this.Index = new Index();
        }

        public bool Check(SHA256 SHA256)
        {
            int i = 0;
            foreach (var c in Commits)
                if (c.Number == i)
                    return false;

            Index index = new Index(this.Commits.Select(c => c.Diff));
            if (!index.Equals(this.Index))
                return false;

            HashValue currentHash = InitialHash;
            foreach (var c in Commits)
            {
                byte[] concat = currentHash * c.Diff.ComputeHash(SHA256);
                if (new HashValue(SHA256.ComputeHash(concat)) != c.Hash)
                    return false;
                currentHash = c.Hash;
            }
            return true;
        }

        public void AddCommit(Commit commit)
        {
            this.Commits.Add(commit);
            this.Index.Apply(commit.Diff);
        }
    }
}
