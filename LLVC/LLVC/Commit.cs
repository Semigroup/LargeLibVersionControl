using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LLVC
{
    public class Commit : IComparable<Commit>
    {
        public HashValue Hash { get;  set; }
        public string Title { get;  set; }
        public string Message { get;  set; }
        public int Number { get;  set; }
        public DateTime TimeStamp { get;  set; }

        public Diff Diff { get; set; }

        private Commit()
        {

        }

        public int CompareTo(Commit other)
        {
            return this.Number - other.Number;
        }

        
    }
}
