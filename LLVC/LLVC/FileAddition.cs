using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LLVC
{
    public class FileAddition : FileUpdate
    {
        public FileAddition(FileEntry File)
           : base(File, Type.Addition)
        {

        }
    }
}
