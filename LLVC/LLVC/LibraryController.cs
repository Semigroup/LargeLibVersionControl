using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using System.Security.Cryptography;

namespace LLVC
{
    public class LibraryController
    {
        public string PathToLibrary { get; private set; }
        public string PathToLLVC { get; private set; }

        public Protocol Protocol { get; private set; }
        public Index FileIndex { get; private set; }

        public SHA256 SHA256 { get; private set; }
        public XmlSerializer Serializer { get; set; }

        public LibraryController(string PathToLibrary)
        {
            this.SHA256 = SHA256.Create();
            this.Serializer = new XmlSerializer(typeof(Protocol));

            this.PathToLibrary = PathToLibrary;
            if (!Directory.Exists(PathToLibrary))
                throw new DirectoryNotFoundException(PathToLibrary + " does not exist!");

            this.PathToLLVC = Path.Combine(PathToLibrary, "/.llvc/");
            if (!Directory.Exists(PathToLLVC))
                throw new DirectoryNotFoundException(PathToLLVC + " does not exist!");

            string protocolFile = Path.Combine(PathToLLVC, "library.protocol");
            if (!File.Exists(protocolFile))
                throw new FileNotFoundException(protocolFile + " does not exist!");

            this.Protocol = (Protocol)Serializer.Deserialize(File.OpenRead(protocolFile));
            if (this.Protocol.Check(SHA256))
                throw new InvalidDataException("library.protocol is not correct!");
        }

        public Index ComputeIndex(string pathToRoot)
        {
            Index index = new Index();

            void traverseDirectory(string relativePath)
            {
                string absolutePath = Path.Combine(pathToRoot, relativePath);
                foreach (string file in Directory.EnumerateFiles(absolutePath))
                    index.FileEntries.Add(relativePath, GetEntry(pathToRoot, Path.Combine(relativePath, file)));
                foreach (string directory in Directory.EnumerateDirectories(absolutePath))
                    if (!directory.StartsWith("."))
                        traverseDirectory(Path.Combine(relativePath, directory));
            }
            return index;
        }

        public FileEntry GetEntry(string pathToRoot, string relativePathToFile)
        {
            string absolutePathToFile = Path.Combine(pathToRoot, relativePathToFile);
            return new FileEntry(relativePathToFile, Hash(absolutePathToFile));
        }

        public HashValue Hash(string absolutePathToFile)
        {
            byte[] bytes;
            using (FileStream fileStream = new FileStream(absolutePathToFile, FileMode.Open))
            {
                fileStream.Position = 0;
                bytes = SHA256.ComputeHash(fileStream);
            }
            return new HashValue(bytes);
        }

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