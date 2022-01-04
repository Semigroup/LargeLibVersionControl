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
                    (x,y) => number++
                );

            return number;
        }

        public static void TraverseFileSystem(string pathToRoot, Action<string, string> fileAction)
            => TraverseFileSystem(pathToRoot, (string absolute, string relative) => { }, fileAction);
        public static void TraverseFileSystem(string pathToRoot, Action<string, string> directoryAction, Action<string, string> fileAction)
        {
            void traverseDirectory(string relativePath)
            {
                string absolutePath = Path.Combine(pathToRoot, relativePath);

                foreach (string filePath in Directory.EnumerateFiles(absolutePath))
                {
                    string file = GetLastIdentifier(filePath);
                    string relativeFilePath = Path.Combine(relativePath, file);
                    fileAction(filePath, relativeFilePath);
                }
                foreach (string directoryPath in Directory.EnumerateDirectories(absolutePath))
                {
                    string directory = GetLastIdentifier(directoryPath);
                    if (!directory.StartsWith("."))
                    {
                        string relativeDirectoryPath = Path.Combine(relativePath, directory);
                        directoryAction(directoryPath, relativeDirectoryPath);
                        traverseDirectory(relativeDirectoryPath);
                    }
                }
            }
            traverseDirectory("");
        }

        public static Index ComputeIndexFromPath(string pathToRoot)
        {
            Index index = new Index();

            TraverseFileSystem(
                    pathToRoot,
                    (absoluteFilePath, relativeFilePath) =>
                    {
                        index.FileEntries.Add(relativeFilePath, new FileEntry(pathToRoot, absoluteFilePath, relativeFilePath));
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
