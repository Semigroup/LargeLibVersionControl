using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

            //check correctness of hash chain
            //check correctness of index
        }

        public void AddCommit(Commit commit)
        {
            this.Commits.Add(commit);
            this.Index.Apply(commit.Diff);
        }
    }
}
