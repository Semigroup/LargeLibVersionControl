using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LLVC
{
    public class CmdLineTool
    {
        public LibraryController Controller { get; set; }

        /// <summary>
        /// select [path] : wählt library an adresse aus, falls vorhanden
        /// init [path] : macht an ort eine neue library, falls nicht bereits vorhanden und fragt nach titel
        /// commit : commited changes und fragt nach titel und message
        /// get : gibt aktuelle Änderungen an
        /// 
        /// 
        /// compareTo [path]
        /// sync : nach compareTo, geht nur, wenn keine uncommitted changes auf dem remote sind
        /// forceSync: geht direkt nach file-indices
        /// 
        /// removeEmptyFolders
        /// replaceWhiteSpaces
        /// 
        /// </summary>
        public void Run()
        {
            while (true)
            {
                string line = Console.ReadLine();
                Console.Out.WriteLine("--> " + line);
            }
        }
    }
}
