﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using System.Security.Cryptography;
using System.Runtime.Serialization;

namespace LLVC
{
    public class LibraryController
    {
        public string PathToLibrary { get; private set; }
        public string PathToLLVC { get; private set; }

        public Protocol Protocol { get; private set; }
        public Index ProtocolIndex { get; private set; }

        public SHA256 SHA256 { get; private set; }
        public XmlSerializer Serializer { get; set; }

        public LibraryController(string PathToLibrary)
        {
            this.SHA256 = SHA256.Create();
            this.Serializer = new XmlSerializer(typeof(Protocol));

            this.PathToLibrary = PathToLibrary;
            if (!Directory.Exists(PathToLibrary))
                throw new DirectoryNotFoundException(PathToLibrary + " does not exist!");

            this.PathToLLVC = Path.Combine(PathToLibrary, ".llvc");
            if (!Directory.Exists(PathToLLVC))
                throw new DirectoryNotFoundException(PathToLLVC + " does not exist!");

            string protocolFile = Path.Combine(PathToLLVC, "library.protocol");
            if (!File.Exists(protocolFile))
                throw new FileNotFoundException(protocolFile + " does not exist!");

            this.Protocol = (Protocol)Serializer.Deserialize(File.OpenRead(protocolFile));
            int number = this.Protocol.CheckNumbering();
            if (number != -1)
                throw new InvalidDataException("library.protocol is not correct!\nCommit No. " + number + " is missing.");

            Commit commit = this.Protocol.CheckHashes(SHA256);
            if (commit != null)
                throw new InvalidDataException("library.protocol is not correct!\n" +
                    "Commit " + commit.Number + ", " + commit.Title + ", has a broken value hash.");

            this.ProtocolIndex = new Index(this.Protocol.Commits.Select(c => c.Diff));
        }

        public Index ComputeIndex(string pathToRoot)
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
                    index.FileEntries.Add(relativePath, GetEntry(pathToRoot, Path.Combine(relativePath, file)));
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

        public Diff GetDiff() => new Diff(this.ProtocolIndex, ComputeIndex(this.PathToLibrary));

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