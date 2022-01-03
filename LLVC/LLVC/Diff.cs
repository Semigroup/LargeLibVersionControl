using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LLVC
{
    public class Diff
    {
        public List<FileUpdate> FileUpdates { get; set; }

        public bool IsEmpty => FileUpdates.Count == 0;

        private Diff()
        {

        }

        public Diff(Index oldIndex, Index newIndex)
        {
            FileUpdates = new List<FileUpdate>();

            foreach (var file in oldIndex.FileEntries.Keys)
            {
                var oldHash = oldIndex[file];
                if (newIndex.FileEntries.ContainsKey(file))
                {
                    var newHash = newIndex[file];
                    if (oldHash != newHash)
                        FileUpdates.Add(new FileUpdate(newIndex.FileEntries[file], FileUpdate.Type.Change));
                }
                else
                    FileUpdates.Add(new FileUpdate(oldIndex.FileEntries[file], FileUpdate.Type.Deletion));
            }
            foreach (var file in newIndex.FileEntries.Keys)
                if (!oldIndex.FileEntries.ContainsKey(file))
                    FileUpdates.Add(new FileUpdate(newIndex.FileEntries[file], FileUpdate.Type.Change));
        }

        public HashValue ComputeHash(HashFunction HashFunction)
        {
            //we assume here there is at least one File Update
            HashValue h = null;
            foreach (var item in FileUpdates)
                h += item.GetHash(HashFunction);
            return h;
        }
    }
}
