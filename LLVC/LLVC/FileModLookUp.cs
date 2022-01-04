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
    public class FileModLookUp : ICloneable
    {
        [DataMember()]
        public SortedDictionary<string, FileEntry> Table { get; set; }

        public FileModLookUp(SortedDictionary<string, FileEntry> Table)
        {
            this.Table = Table;
        }
        public FileModLookUp(): this(new SortedDictionary<string, FileEntry>())
        {
        }

        public void Update(Commit commit)
        {
            foreach (var item in commit.Diff.FileUpdates)
            {
                var entry = item.File;
                Table[entry.RelativePath] = entry;
            }
        }

        public object Clone()
        {
            var table = new SortedDictionary<string, FileEntry>();
            foreach (var item in Table)
                table.Add(item.Key, item.Value.Clone() as FileEntry);
            return new FileModLookUp(table);
        }
    }
}
