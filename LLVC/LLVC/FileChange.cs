using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LLVC
{
    public class FileChange : FileUpdate
    {
        public HashValue OldFileHash { get; set; }

        public FileChange(FileEntry File)
          : base(File, Type.Change)
        {

        }
    }
}
