using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LLVC
{
    public class FileMovement : FileUpdate
    {
        public string OldRelativePath { get; set; }

        public FileMovement(FileEntry File) 
            : base(File, Type.Movement)
        {

        }
    }
}
