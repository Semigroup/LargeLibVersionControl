using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LLVC
{
    public class Commit : IComparable<Commit>, ICloneable
    {
        public int Number { get; set; }
        public string Title { get; set; }
        public string Message { get; set; }
        public DateTime TimeStamp { get; set; }
        public HashValue Hash { get; set; }
        public Diff Diff { get; set; }

        private Commit()
        {

        }

        public Commit(int Number, string Title, string Message, DateTime TimeStamp, HashValue Hash, Diff Diff)
        {
            this.Number = Number;
            this.Title = Title;
            this.Message = Message;
            this.TimeStamp = TimeStamp;
            this.Diff = Diff;
            this.Hash = Hash;
            this.Diff = Diff;
        }

        public int CompareTo(Commit other)
        {
            return this.Number - other.Number;
        }

        public void PurgeAbsolutePaths()
        {
            foreach (var item in Diff.FileUpdates)
            {
                var entry = item.File;
                entry.AbsolutePath = null;
                entry.PathToRoot = null;
            }
        }

        public object Clone()
            => new Commit()
            {
                Number = Number,
                Title = Title,
                Message = Message,
                TimeStamp = TimeStamp,
                Hash = Hash,
                Diff = Diff.Clone() as Diff
            };
    }
}
