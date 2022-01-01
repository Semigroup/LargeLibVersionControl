using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LLVC
{
    public class Commit : IComparable<Commit>
    {
        public IList<FileUpdate> FileUpdates { get; set; }

        public HashValue Hash { get; private set; }
        public string Name { get; private set; }
        public int Number { get; private set; }
        public DateTime TimeStamp { get; private set; }

        public int CompareTo(Commit other)
        {
            return this.Number - other.Number;
        }
    }
}
