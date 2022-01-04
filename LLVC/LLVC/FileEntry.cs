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
        public string RelativePath { get; set; }
        public string PathToRoot { get; set; }
        public string AbsolutePath { get; set; }
        public DateTime LastWrittenTime { get; set; }
        public long Size { get; set; }
        public HashValue FileHash { get; set; }

        private FileEntry()
        {

        }

        public FileEntry(string PathToRoot, string AbsolutePath, string RelativePath)
        {
            this.PathToRoot = PathToRoot;
            this.AbsolutePath = AbsolutePath;
            this.RelativePath = RelativePath;

            FileInfo info = new FileInfo(AbsolutePath);
            this.LastWrittenTime = info.LastWriteTime;
            this.Size = info.Length;
        }

        public void ComputeHash(HashFunction hashFunction)
        {
            if (FileHash is null)
                FileHash = hashFunction.ComputeHash(AbsolutePath);
        }

        //public override bool Equals(object obj)
        //{
        //    if (!(obj is FileEntry entry))
        //        return false;
        //    return (this.RelativePath == entry.RelativePath) && (this.FileHash == entry.FileHash);
        //}

        //public override int GetHashCode()
        //{
        //    return RelativePath.GetHashCode() + FileHash.GetHashCode();
        //}
    }
}
