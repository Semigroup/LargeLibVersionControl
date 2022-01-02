using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LLVC
{
    public class HashValue
    {
        public byte[] Bytes { get; private set; }

        public HashValue(byte[] Bytes)
        {
            this.Bytes = Bytes;
        }

        public string GetBase64()
        {
            return "=" + Convert.ToBase64String(Bytes, Base64FormattingOptions.None) + "=";
        }
    }
}
