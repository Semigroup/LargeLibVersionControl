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
            foreach (var entry in libraryController.ProtocolIndex.FileEntries.Values)
            {
                string source = Path.Combine(libraryController.PathToLibrary, entry.RelativePath);
                string target = Path.Combine(targetPath, entry.RelativePath);
                Directory.CreateDirectory(target);
                File.Copy(source, target, true);
            }

            var newController = libraryController.Copy(targetPath, newLibraryName);
            newController.SaveProtocol();
            newController.SaveLookUpTable();
        }
    }
}
