using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LLVC
{
    public class Diff : ICloneable
    {
        public List<FileUpdate> FileUpdates { get; set; }

        public bool IsEmpty => FileUpdates.Count == 0;

        private Diff()
        {
        }
        public Diff(List<FileUpdate> FileUpdates)
        {
            this.FileUpdates = FileUpdates;
        }

        public void AddDeletion(FileEntry missingFile)
            => this.FileUpdates.Add(new FileUpdate(missingFile, FileUpdate.Type.Deletion));
        public void AddChange(FileEntry changedFile)
            => this.FileUpdates.Add(new FileUpdate(changedFile, FileUpdate.Type.Change));

        public (List<FileEntry> additions, List<FileEntry> changes, List<FileEntry> deletions )
            SortUpdates()
        {
            List<FileEntry> deletions = new List<FileEntry>();
            List<FileEntry> changes = new List<FileEntry>();
            List<FileEntry> additions = new List<FileEntry>();
            foreach (var update in FileUpdates)
                switch (update.MyType)
                {
                    case FileUpdate.Type.Deletion:
                        deletions.Add(update.File);
                        break;
                    case FileUpdate.Type.Change:
                        if (update.File.FileHash is null)
                            additions.Add(update.File);
                        else
                            changes.Add(update.File);
                        break;
                    default:
                        throw new NotImplementedException();
                }
            return (additions, changes, deletions);
        }

        public HashValue ComputeHash(HashFunction HashFunction)
        {
            //we assume here there is at least one File Update
            HashValue h = null;
            foreach (var item in FileUpdates)
                h += item.GetHash(HashFunction);
            return h;
        }

        public object Clone()
            => new Diff(this.FileUpdates.Select(x => x.Clone() as FileUpdate).ToList());
    }
}
