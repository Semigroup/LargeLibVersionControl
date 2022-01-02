using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LLVC
{
    public class Commit : IComparable<Commit>
    {
        public HashValue Hash { get; private set; }
        public string Title { get; private set; }
        public string Message { get; private set; }
        public int Number { get; private set; }
        public DateTime TimeStamp { get; private set; }

        public Diff Diff { get; set; }

        public int CompareTo(Commit other)
        {
            return this.Number - other.Number;
        }
    }
}
