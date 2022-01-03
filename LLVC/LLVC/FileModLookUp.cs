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
    public class FileModLookUp
    {
        [DataMember()]
        public SortedDictionary<string, FileEntry> Table { get; set; }

        public FileModLookUp()
        {
            Table = new SortedDictionary<string, FileEntry>();
        }

        public List<FileEntry> GetLastWrittenFiles(string pathToRoot)
        {
            List<FileEntry> writtenFiles = new List<FileEntry>();
            long totalSize = 0;

            void fileAction(string absolutePath, string relativeFilePath)
            {
                FileEntry file = new FileEntry(pathToRoot, absolutePath, relativeFilePath);
                file.ComputeInfo();
                bool changed = !Table.ContainsKey(relativeFilePath)
                    || Table[relativeFilePath].LastWrittenTime != file.LastWrittenTime;
                if (changed)
                {
                    writtenFiles.Add(file);
                    totalSize += file.Size;
                }
            }

            FileHelper.TraverseFileSystem(pathToRoot, fileAction);
            return writtenFiles;
        }

        public Diff GetChangedFiles(HashFunction hashFunction, string pathToRoot)
        {
            Diff diff = new Diff(new List<FileUpdate>());

            foreach (var fileEntry in Table.Values)
                if (!File.Exists(fileEntry.AbsolutePath))
                    diff.AddDeletion(fileEntry);

            var changedFiles = GetLastWrittenFiles(pathToRoot);

            long totalSize = changedFiles.Select(entry => entry.Size).Sum();
            bool printProgress = totalSize >= (1 << 27);

            var coroutine = CmdLineTool.PrintHashProgress(totalSize, changedFiles).GetEnumerator();
            foreach (var entry in changedFiles)
            {
                if (printProgress)
                    coroutine.MoveNext();

                if (Table.ContainsKey(entry.RelativePath))
                {
                    entry.ComputeHash(hashFunction);
                    if (Table[entry.RelativePath].FileHash != entry.FileHash)
                        diff.AddChange(entry);
                    else
                        Table[entry.RelativePath] = entry;
                }
                else
                    diff.AddChange(entry);
            }

            return diff;
        }
    }
}
