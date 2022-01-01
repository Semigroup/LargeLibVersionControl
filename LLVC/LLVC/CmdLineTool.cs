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
        /// commit : commited changes und fragt nach titel
        /// (kritische zeichen aus titel entfernen: \n \r #)
        /// 
        /// compareTo [path]
        /// sync : nach compareTo
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
