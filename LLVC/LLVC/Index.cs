using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LLVC
{
    public class Index
    {
        public IDictionary<string, FileEntry> FileEntries { get; private set; }

        public Index(IDictionary<string, FileEntry> FileEntries)
        {
            this.FileEntries = FileEntries;
        }
        public Index() : this(new SortedDictionary<string, FileEntry>())
        {

        }

        public void Apply(Diff diff)
        {
            foreach (var update in diff.FileUpdates)
            {
                if (!FileEntries.ContainsKey(update.File.RelativePath))
                    throw new KeyNotFoundException(update.File.RelativePath + " not found in index!");

                switch (update.MyType)
                {
                    case FileUpdate.Type.Deletion:
                        FileEntries.Remove(update.File.RelativePath);
                        break;
                    case FileUpdate.Type.Change:
                        FileEntries[update.File.RelativePath] = update.File;
                        break;
                    default:
                        throw new NotImplementedException();
                }
            }
        }
    }
}
