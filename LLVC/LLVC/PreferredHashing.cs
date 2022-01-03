using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.IO;
using Force.Crc32;

namespace LLVC
{
    public class HashFunction
    {
        public HashFunction()
        {
        }

        public HashValue ComputeHash(byte[] bytes)
        {
            return new HashValue(Crc32Algorithm.Compute(bytes));
        }

        public HashValue ComputeHash(string absolutePath)
        {
            uint hash = 0;
            using (FileStream fs = File.OpenRead(absolutePath))
            {
                byte[] buff = new byte[1024];
                while (fs.Length != fs.Position)
                {
                    int count = fs.Read(buff, 0, buff.Length);
                    hash = Crc32Algorithm.Append(hash, buff, 0, count);
                }
            }
            return new HashValue(hash);

        }
    }
}
