using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;

namespace LLVC
{
   public class FileUpdate
    {
        public enum Type
        {
            Deletion = 1,
            Change = 2,
        }

        public FileEntry File { get;  set; }
        public Type MyType { get;  set; }

        private FileUpdate()
        {

        }

        public FileUpdate(FileEntry File, Type MyType)
        {
            this.File = File;
            this.MyType = MyType;
        }

        public HashValue GetHash(SHA256 SHA256)
        {
            int n = Encoding.UTF8.GetByteCount(File.RelativePath);
            byte[] buffer = new byte[1 + n + File.FileHash.Bytes.Length];
            buffer[0] = (byte)MyType;
            Encoding.UTF8.GetBytes(File.RelativePath, 0, File.RelativePath.Length, buffer, 1);
            File.FileHash.Bytes.CopyTo(buffer, 1 + n);

            return new HashValue(SHA256.ComputeHash(buffer));
        }
    }
}
