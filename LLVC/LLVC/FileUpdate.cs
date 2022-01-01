using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LLVC
{
   public class FileUpdate
    {
        public enum Type
        {
            Addition = 1,
            Change = 2,
            Movement = 4,
            Deletion = 8
        }

        public FileEntry File { get; private set; }
        public Type MyType { get; private set; }

        public FileUpdate(FileEntry File, Type MyType)
        {
            this.File = File;
            this.MyType = MyType;
        }
    }
}
