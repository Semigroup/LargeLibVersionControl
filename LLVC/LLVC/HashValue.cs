using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LLVC
{
    public class HashValue
    {
        public byte[] Bytes { get; set; }

        private HashValue()
        {

        }

        public HashValue(byte[] Bytes)
        {
            this.Bytes = Bytes;
        }

        public string GetBase64()
        {
            return "=" + Convert.ToBase64String(Bytes, Base64FormattingOptions.None) + "=";
        }

        public static bool operator ==(HashValue h1, HashValue h2)
        {
            if (h1 == null)
                return h2 == null;
            if (h2 == null)
                return false;

            if (h1.Bytes == null)
                return h2.Bytes == null;
            if (h2.Bytes == null)
                return false;

            if (h1.Bytes.Length != h2.Bytes.Length)
                return false;

            for (int i = 0; i < h1.Bytes.Length; i++)
                if (h1.Bytes[i] != h2.Bytes[i])
                    return false;

            return true;
        }

        public static bool operator !=(HashValue h1, HashValue h2) => !(h1 == h2);

        public static HashValue operator +(HashValue h1, HashValue h2)
        {
            if (h1 == null)
                return h2;
            if (h2 == null)
                return h1;

            byte[] xored = new byte[h1.Bytes.Length];
            for (int i = 0; i < xored.Length; i++)
                xored[i] = (byte)(h1.Bytes[i] ^ h2.Bytes[i]);
            return new HashValue(xored);
        }

        public static byte[] operator *(HashValue h1, HashValue h2)
        {
            byte[] concatenation = new byte[h1.Bytes.Length + h2.Bytes.Length];
            h1.Bytes.CopyTo(concatenation, 0);
            h2.Bytes.CopyTo(concatenation, h1.Bytes.Length);
            return concatenation;
        }


        public override bool Equals(object obj)
        {
            return this == (obj as HashValue);
        }

        public override int GetHashCode()
        {
            int value = 0;
            for (int i = 0; i < Bytes.Length; i ++)
            {
                value = (value >> 8) + (value << 24);
                value += Bytes[i];
            }
            return value;
        }

        public override string ToString() => GetBase64();
    }
}
