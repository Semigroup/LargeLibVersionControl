using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace LLVC
{
    public static class FileHelper
    {
        public static long CountFiles(string pathToRoot)
        {
            long number = 0;

            TraverseFileSystem(pathToRoot,
                    relativeDirectoryPath => { },
                    relativeFilePath => number++
                );

            return number;
        }

        public static void TraverseFileSystem(string pathToRoot, Action<string> fileAction)
            => TraverseFileSystem(pathToRoot, directory => { }, fileAction);
        public static void TraverseFileSystem(string pathToRoot, Action<string> directoryAction, Action<string> fileAction)
        {
            void traverseDirectory(string relativePath)
            {
                string absolutePath = Path.Combine(pathToRoot, relativePath);

                foreach (string filePath in Directory.EnumerateFiles(absolutePath))
                {
                    string file = GetLastIdentifier(filePath);
                    string relativeFilePath = Path.Combine(relativePath, file);
                    fileAction(relativeFilePath);
                }
                foreach (string directoryPath in Directory.EnumerateDirectories(absolutePath))
                {
                    string directory = GetLastIdentifier(directoryPath);
                    string relativeDirectoryPath = Path.Combine(relativePath, directory);
                    directoryAction(relativeDirectoryPath);
                    if (!directory.StartsWith("."))
                        traverseDirectory(relativeDirectoryPath);
                }
            }
            traverseDirectory("");
        }

        public static Index ComputeIndexFromPath(HashFunction hashFunction, string pathToRoot, Action<string> statusUpdateFunction)
        {
            Index index = new Index();

            TraverseFileSystem(
                    pathToRoot,
                    relativeDirectoryPath => { },
                    relativeFilePath =>
                    {
                        statusUpdateFunction(relativeFilePath);
                        index.FileEntries.Add(relativeFilePath, new FileEntry(hashFunction, pathToRoot, relativeFilePath));
                    }
                );

            return index;
        }

        public static string GetLastIdentifier(string path)
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


    }
}
