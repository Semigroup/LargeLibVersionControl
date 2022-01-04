using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using System.Runtime.Serialization;

namespace LLVC
{
    public class LibraryController
    {
        public string PathToLibrary { get; private set; }
        public string PathToLLVC { get; private set; }
        public string PathToProtocolFile { get; private set; }
        public string PathToLookUp { get; private set; }

        public Protocol Protocol { get; private set; }
        public Index ProtocolIndex { get; private set; }
        public FileModLookUp LookUp { get; private set; }

        public HashFunction HashFunction { get; private set; }
        public XmlSerializer Serializer { get; private set; }
        public DataContractSerializer DataSerializer { get; private set; }

        public LibraryController(string PathToLibrary)
        {
            this.HashFunction = new HashFunction();
            this.Serializer = new XmlSerializer(typeof(Protocol));
            this.DataSerializer = new DataContractSerializer(typeof(FileModLookUp));

            this.PathToLibrary = PathToLibrary;
            if (!Directory.Exists(PathToLibrary))
                throw new DirectoryNotFoundException(PathToLibrary + " does not exist!");

            this.PathToLLVC = Path.Combine(PathToLibrary, ".llvc");
            if (!Directory.Exists(PathToLLVC))
                throw new DirectoryNotFoundException(PathToLLVC + " does not exist!");

            this.PathToProtocolFile = Path.Combine(PathToLLVC, "library.protocol");
            if (!File.Exists(PathToProtocolFile))
                throw new FileNotFoundException(PathToProtocolFile + " does not exist!");
            ReadProtocol();
            this.ProtocolIndex = new Index(this.Protocol.Commits.Select(c => c.Diff));

            this.PathToLookUp = Path.Combine(PathToLLVC, "lastModified.lookUpTable");
            if (!File.Exists(PathToLookUp))
                throw new FileNotFoundException(PathToLookUp + " does not exist!");
            this.ReadLookUpTable();
        }
        private LibraryController(string PathToLibrary, string libraryName, byte[] seed)
        {
            this.HashFunction = new HashFunction();
            this.Serializer = new XmlSerializer(typeof(Protocol));
            this.DataSerializer = new DataContractSerializer(typeof(FileModLookUp));

            this.PathToLibrary = PathToLibrary;
            this.PathToLLVC = Path.Combine(PathToLibrary, ".llvc");
            this.PathToProtocolFile = Path.Combine(PathToLLVC, "library.protocol");
            this.PathToLookUp = Path.Combine(PathToLLVC, "lastModified.lookUpTable");

            this.Protocol = new Protocol(libraryName, this.HashFunction.ComputeHash(seed));
            this.ProtocolIndex = new Index(this.Protocol.Commits.Select(c => c.Diff));
            SaveProtocol();

            this.LookUp = new FileModLookUp();
            this.SaveLookUpTable();
        }

        public void SaveProtocol()
        {
            using (var openStream = File.OpenWrite(PathToProtocolFile))
                Serializer.Serialize(openStream, this.Protocol);
        }
        public void ReadProtocol()
        {
            using (var openStream = File.OpenRead(PathToProtocolFile))
                this.Protocol = (Protocol)Serializer.Deserialize(openStream);

            int number = this.Protocol.CheckNumbering();
            if (number != -1)
                throw new InvalidDataException("library.protocol is not correct!\nCommit No. " + number + " is missing.");

            Commit commit = this.Protocol.CheckHashes(HashFunction);
            if (commit != null)
                throw new InvalidDataException("library.protocol is not correct!\n" +
                    "Commit " + commit.Number + ", " + commit.Title + ", has a broken hash value.");
        }
        public void SaveLookUpTable()
        {
            using (var fs = File.OpenWrite(PathToLookUp))
            using (var dictWriter = XmlDictionaryWriter.CreateTextWriter(fs, Encoding.UTF8))
                DataSerializer.WriteObject(fs, this.LookUp);
        }
        public void ReadLookUpTable()
        {
            using (var fs = File.OpenRead(PathToLookUp))
            using (var dictReader = XmlDictionaryReader.CreateTextReader(fs, new XmlDictionaryReaderQuotas()))
                this.LookUp = (FileModLookUp)DataSerializer.ReadObject(dictReader);
        }

        public void Commit(string title, string message, DateTime timeStamp, Diff diff)
        {
            int number;
            HashValue hash;
            if (this.Protocol.Commits.Count > 0)
            {
                Commit lastCommit = this.Protocol.Commits.Last();
                hash = lastCommit.Hash;
                number = lastCommit.Number + 1;
            }
            else
            {
                hash = this.Protocol.InitialHash;
                number = 0;
            }
            hash = Protocol.Concat(HashFunction, hash, diff.ComputeHash(HashFunction));

            Commit c = new Commit(number, title, message, timeStamp, hash, diff);
            Protocol.Commits.Add(c);
            this.ProtocolIndex.Apply(diff);

            SaveProtocol();
        }

        public Diff GetQuickDiff()
        {
            bool isDirty(FileEntry newEntry)
            {
                return LookUp.Table[newEntry.RelativePath].LastWrittenTime != newEntry.LastWrittenTime;
            }
            void entryConflictResolved(FileEntry oldEntry, FileEntry newEntry)
            {
                if (oldEntry.FileHash == newEntry.FileHash)
                    LookUp.Table[newEntry.RelativePath] = newEntry;
            }

            Diff diff = ComputeDiff(isDirty, entryConflictResolved);
            SaveLookUpTable();

            return diff;
        }
            
        public Diff GetFullDiff()
        {
            bool isDirty(FileEntry newEntry) => true;

            void entryConflictResolved(FileEntry oldEntry, FileEntry newEntry)
            {
                if (oldEntry.FileHash == newEntry.FileHash)
                    LookUp.Table[newEntry.RelativePath] = newEntry;
            }

            Diff diff = ComputeDiff(isDirty, entryConflictResolved);
            SaveLookUpTable();

            return diff;
        }

        public Diff ComputeDiff(Predicate<FileEntry> isDirty, Action<FileEntry, FileEntry> entryConflictResolved)
        {
            var fileIndex = FileHelper.ComputeIndexFromPath(PathToLibrary);
            Diff diff = new Diff(new List<FileUpdate>());

            var needToBeCompared = new List<(FileEntry oldEntry, FileEntry newEntry)>();

            foreach (var oldEntry in ProtocolIndex.FileEntries.Values)
                if (!fileIndex.FileEntries.ContainsKey(oldEntry.RelativePath))
                    diff.AddDeletion(oldEntry);

            foreach (var newEntry in fileIndex.FileEntries.Values)
                if (ProtocolIndex.FileEntries.TryGetValue(newEntry.RelativePath, out FileEntry oldEntry))
                {
                    if (isDirty(newEntry))
                        needToBeCompared.Add((oldEntry, newEntry));
                }
                else
                    diff.AddChange(newEntry);

            var coroutine = CmdLineTool.PrintHashProgress(
                    1 << 27,
                    needToBeCompared.Select(a => a.newEntry)
                ).GetEnumerator();
            foreach ((var oldEntry, var newEntry) in needToBeCompared)
            {
                coroutine.MoveNext();

                newEntry.ComputeHash(HashFunction);
                if (oldEntry.FileHash != newEntry.FileHash)
                    diff.AddChange(newEntry);
                entryConflictResolved(oldEntry, newEntry);
            }

            return diff;
        }

        public static LibraryController Create(string absolutePathToRoot, string libraryName, byte[] seed)
        {
            if (!Directory.Exists(absolutePathToRoot))
                Directory.CreateDirectory(absolutePathToRoot);

            string absolutePathToLLVC = Path.Combine(absolutePathToRoot, ".llvc");
            Directory.CreateDirectory(absolutePathToLLVC);

            return new LibraryController(absolutePathToRoot, libraryName, seed);
        }
    }
}