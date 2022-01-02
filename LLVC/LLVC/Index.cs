using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LLVC
{
    public class Index
    {
        public SortedDictionary<string, FileEntry> FileEntries { get; private set; }

        public Index(SortedDictionary<string, FileEntry> FileEntries)
        {
            this.FileEntries = FileEntries;
        }
        public Index() : this(new SortedDictionary<string, FileEntry>())
        {

        }
        public Index(IEnumerable<Diff> diffs) : this()
        {
            foreach (var diff in diffs)
                this.Apply(diff);
        }

        public HashValue this[string relativeFilePath]
        {
            get => FileEntries[relativeFilePath].FileHash;
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

        public override bool Equals(object obj)
        {
            if (!(obj is Index index))
                return false;
            if (this.FileEntries == null)
                return index.FileEntries == null;
            if (index.FileEntries == null)
                return false;

            bool subset(IDictionary<string, FileEntry> A, IDictionary<string, FileEntry> B)
            {
                foreach (var key in A.Keys)
                {
                    if (!B.ContainsKey(key))
                        return false;
                    if (!A[key].Equals(B[key]))
                        return false;
                }
                return true;
            }
            return subset(this.FileEntries, index.FileEntries) 
                && subset(index.FileEntries, this.FileEntries);
        }

        public override int GetHashCode()
        {
            int value = 0;
            foreach (var item in FileEntries.Keys)
                value += item.GetHashCode();
            foreach (var item in FileEntries.Values)
                value += item.GetHashCode();
            return value;
        }
    }
}
