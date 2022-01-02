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
        public string RelativePath { get;  set; }
        public HashValue FileHash { get;  set; }

        private FileEntry()
        {

        }

        public FileEntry(string RelativePath, HashValue FileHash)
        {
            this.RelativePath = RelativePath;
            this.FileHash = FileHash;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is FileEntry entry))
                return false;
            return (this.RelativePath == entry.RelativePath) && (this.FileHash == entry.FileHash);
        }

        public override int GetHashCode()
        {
            return RelativePath.GetHashCode() + FileHash.GetHashCode();
        }
    }
}
