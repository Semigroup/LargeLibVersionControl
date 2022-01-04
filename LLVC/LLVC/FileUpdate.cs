using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LLVC
{
   public class FileUpdate : ICloneable
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

        public HashValue GetHash(HashFunction HashFunction)
        {
            int n = Encoding.UTF8.GetByteCount(File.RelativePath);
            File.ComputeHash(HashFunction);
            byte[] buffer = new byte[1 + n + File.FileHash.Bytes.Length];
            buffer[0] = (byte)MyType;
            Encoding.UTF8.GetBytes(File.RelativePath, 0, File.RelativePath.Length, buffer, 1);
            File.FileHash.Bytes.CopyTo(buffer, 1 + n);

            return HashFunction.ComputeHash(buffer);
        }

        public object Clone()
             => new FileUpdate()
             {
                 File = File.Clone() as FileEntry,
                 MyType = MyType
             };
    }
}
