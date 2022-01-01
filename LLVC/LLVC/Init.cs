using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LLVC
{
   public class Init
    {
        public HashValue InitialHash { get; private set; }
        public string LibraryName { get; private set; }

        public Init(string LibraryName, HashValue InitialHash)
        {
            this.LibraryName = LibraryName;
            this.InitialHash = InitialHash;
        }
    }
}
