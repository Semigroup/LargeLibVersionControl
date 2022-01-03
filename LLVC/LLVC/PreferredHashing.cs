using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.IO;
//using Force.Crc32;

namespace LLVC
{
    public class HashFunction
    {
        public HashAlgorithm Algorithm { get; set; }

        public HashFunction()
        {
            Algorithm = SHA256.Create();
        }

        //private byte Sum(byte[] bytes)
        //{
        //    byte result = 0;
        //    for (int i = 0; i < bytes.Length; i++)
        //        result += bytes[i];
        //    return result;
        //}

        public HashValue ComputeHash(byte[] bytes)
        {
            return new HashValue(Algorithm.ComputeHash(bytes));
            //return new HashValue(Sum(bytes));
        }

        public HashValue ComputeHash(string absolutePath)
        {
            HashValue h;
            using (var fs = File.OpenRead(absolutePath))
                h = new HashValue(Algorithm.ComputeHash(fs));
            return h;
            //uint hash = 0;
            //using (FileStream fs = File.OpenRead(absolutePath))
            //{
            //    byte[] buff = new byte[1024];
            //    while (fs.Length != fs.Position)
            //    {
            //        int count = fs.Read(buff, 0, buff.Length);
            //        hash = Crc32CAlgorithm.Append(hash, buff, 0, count);
            //    }
            //}
            //return new HashValue(hash);

            //byte result = 0;
            //using (FileStream fs = File.OpenRead(absolutePath))
            //{
            //    byte[] buff = new byte[1024];
            //    while (fs.Length != fs.Position)
            //    {
            //        int count = fs.Read(buff, 0, buff.Length);
            //        result += Sum(buff);
            //    }
            //}
            //return new HashValue(result);
        }
    }
}
