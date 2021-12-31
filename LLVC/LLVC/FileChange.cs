using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LLVC
{
    public class FileChange : FileUpdate
    {
        public byte[] OldHash { get; set; }
    }
}
