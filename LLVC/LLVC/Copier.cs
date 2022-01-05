using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace LLVC
{
    public class Copier
    {
        public static void CopyTo(LibraryController libraryController, string targetPath, string newLibraryName)
        {
            if (!Directory.Exists(targetPath))
                Directory.CreateDirectory(targetPath);

            //Queue<Task> tasks = new Queue<Task>();

            CmdLineTool.WorkOnFiles(
                libraryController.ProtocolIndex.FileEntries.Values,
                "Copy",
                entry =>
                {
                    string source = Path.Combine(libraryController.PathToLibrary, entry.RelativePath);
                    string target = Path.Combine(targetPath, entry.RelativePath);
                    Directory.CreateDirectory(Path.GetDirectoryName(target));
                    File.Copy(source, target, true);
                    //tasks.Enqueue(CopyFileAsync(source, target));
                }
                );

            var newController = libraryController.Copy(targetPath, newLibraryName);
            Directory.CreateDirectory(newController.PathToLLVC);
            newController.SaveProtocol();
            newController.SaveLookUpTable();
        }

        public static async Task CopyFileAsync(string sourceFile, string destinationFile)
        {
            var buffersize = 1 << 20;
            using (var sourceStream = new FileStream(sourceFile, FileMode.Open, FileAccess.Read, FileShare.Read, buffersize, FileOptions.Asynchronous | FileOptions.SequentialScan))
            using (var destinationStream = new FileStream(destinationFile, FileMode.Create, FileAccess.Write, FileShare.None, buffersize, FileOptions.Asynchronous | FileOptions.SequentialScan))
                await sourceStream.CopyToAsync(destinationStream, buffersize);
        }
    }
}
