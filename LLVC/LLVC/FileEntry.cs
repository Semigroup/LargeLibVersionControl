using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace LLVC
{
   public class FileEntry
    {
        public string RelativePath { get; private set; }
        public HashValue FileHash { get; private set; }

        public FileEntry(string RelativePath, HashValue FileHash)
        {
            this.RelativePath = RelativePath;
            this.FileHash = FileHash;
        }
    }
}
