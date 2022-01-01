using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LLVC
{
   public class FileEntry
    {
        public string RelativePath { get; set; }
        public HashValue FileHash { get; set; }
    }
}
