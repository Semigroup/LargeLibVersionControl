using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace LLVC
{
    public class LibraryController
    {
        public string PathToLibrary { get; private set; }
        public string PathToLLVC { get; private set; }
        public string PathToProtocolFile { get; private set; }

        public Protocol Protocol { get; private set; }
        public Index ProtocolIndex { get; private set; }

        public HashFunction HashFunction { get; private set; }
        public XmlSerializer Serializer { get; set; }

        public LibraryController(string PathToLibrary)
        {
            this.HashFunction = new HashFunction();
            this.Serializer = new XmlSerializer(typeof(Protocol));

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
        public Index ComputeIndex(string pathToRoot, Action<string> statusUpdateFunction)
        {
            Index index = new Index();

            void traverseDirectory(string relativePath)
            {
                string absolutePath = Path.Combine(pathToRoot, relativePath);

                //Console.WriteLine("pathToRoot: " + pathToRoot);
                //Console.WriteLine("relativePath: " + relativePath);
                //Console.WriteLine("absolutePath: " + absolutePath);

                //Console.WriteLine("Enumerating files:");
                foreach (string filePath in Directory.EnumerateFiles(absolutePath))
                {
                    string file = GetLastIdentifier(filePath);
                    //Console.WriteLine(file);
                    string relativeFilePath = Path.Combine(relativePath, file);
                    statusUpdateFunction(relativeFilePath);
                    index.FileEntries.Add(relativeFilePath, GetEntry(pathToRoot, relativeFilePath));
                }

                //Console.WriteLine("Enumerating Directories:");
                foreach (string directoryPath in Directory.EnumerateDirectories(absolutePath))
                {
                    string directory = GetLastIdentifier(directoryPath);
                    //Console.WriteLine(directory);
                    if (!directory.StartsWith("."))
                        traverseDirectory(Path.Combine(relativePath, directory));
                }
            }

            traverseDirectory("");

            return index;
        }
        public long CountFiles(string pathToRoot)
        {
            long number = 0;

            void traverseDirectory(string relativePath)
            {
                string absolutePath = Path.Combine(pathToRoot, relativePath);
                foreach (string filePath in Directory.EnumerateFiles(absolutePath))
                    number++;
                foreach (string directoryPath in Directory.EnumerateDirectories(absolutePath))
                {
                    string directory = GetLastIdentifier(directoryPath);
                    if (!directory.StartsWith("."))
                        traverseDirectory(Path.Combine(relativePath, directory));
                }
            }
            traverseDirectory("");

            return number;
        }
        public string GetLastIdentifier(string path)
        {
            int length = 0;
            for (int i = path.Length - 1; i >= 0; i--)
            {
                if (path[i] == '\\')
                {
                    if (length > 0)
                        return path.Substring(i + 1, length);
                }
                else
                    length++;
            }

            return "";
        }

        public FileEntry GetEntry(string pathToRoot, string relativePathToFile)
        {
            string absolutePathToFile = Path.Combine(pathToRoot, relativePathToFile);
            return new FileEntry(relativePathToFile, HashFunction.ComputeHash(absolutePathToFile));
        }

        public Diff GetDiff(Action<string> statusUpdateFunction)
            => new Diff(this.ProtocolIndex, ComputeIndex(this.PathToLibrary, statusUpdateFunction));

        public static void Create(string absolutePathToRoot, string libraryName, HashValue initialHash)
        {
            if (!Directory.Exists(absolutePathToRoot))
                Directory.CreateDirectory(absolutePathToRoot);

            string absolutePathToLLVC = Path.Combine(absolutePathToRoot, ".llvc");
            Directory.CreateDirectory(absolutePathToLLVC);

            Protocol protocol = new Protocol(libraryName, initialHash);
            XmlSerializer serializer = new XmlSerializer(typeof(Protocol));
            using (var fs = new FileStream(Path.Combine(absolutePathToLLVC, "library.protocol"), FileMode.CreateNew))
                serializer.Serialize(fs, protocol);
        }
    }
}