using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LLVC
{
    public class Commit
    {
        public IList<FileUpdate> FileUpdates { get; set; }

        public HashValue Hash { get; set; }

        public string Name { get; set; }

    }
}
