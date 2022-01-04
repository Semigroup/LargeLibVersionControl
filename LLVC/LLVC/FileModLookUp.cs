using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using System.IO;

namespace LLVC
{
    [DataContract()]
    public class FileModLookUp
    {
        [DataMember()]
        public SortedDictionary<string, FileEntry> Table { get; set; }

        public FileModLookUp()
        {
            Table = new SortedDictionary<string, FileEntry>();
        }
    }
}
