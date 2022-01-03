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
        public SortedDictionary<string, (DateTime timeStamp, HashValue hash)> Table { get; set; }

        public FileModLookUp()
        {
            Table = new SortedDictionary<string, (DateTime timeStamp, HashValue hash)>();
        }

        public (long size, List<string> writtenFiles) GetNumberSizeWrittenFiles(string pathToRoot)
        {
            List<string> writtenFiles = new List<string>();
            long size = 0;

            void fileAction(string relativeFilePath)
            {
                var info = new FileInfo(Path.Combine(pathToRoot, relativeFilePath));
                bool notChanged = Table.ContainsKey(relativeFilePath)
                    && (Table[relativeFilePath].timeStamp == info.LastWriteTime);
                if (!notChanged)
                {
                    writtenFiles.Add(relativeFilePath);
                    size += info.Length;
                }
            }

            FileHelper.TraverseFileSystem(pathToRoot, d => { }, fileAction);
            return (size, writtenFiles);
        }

        public Diff GetChangedFiles(HashFunction hashFunction, string pathToRoot, Index protocolIndex)
        {
            Diff diff = new Diff(new List<FileUpdate>());

            foreach (var entry in protocolIndex.FileEntries.Values)
                if (!File.Exists(Path.Combine(pathToRoot, entry.RelativePath)))
                    diff.AddDeletion(entry);

            (long size, List<string> writtenFiles) = GetNumberSizeWrittenFiles(pathToRoot);

            bool printProgress = size >= (1 << 27);

            void fileAction(string relativeFilePath)
            {
                if (true)
                {

                }
            }
            FileHelper.TraverseFileSystem(pathToRoot)

            return diff;
        }
    }
}
