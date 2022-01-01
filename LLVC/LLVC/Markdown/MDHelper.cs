using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace LLVC.Markdown
{
    public class MDHelper
    {
        public int TabWidth { get; set; } = 4;

        public void WriteToFile(Init init, string filePath)
        {
            StreamWriter writer = new StreamWriter(filePath, false, Encoding.UTF8);
            writer.WriteLine("# Init");
            writer.WriteLine("Name: " + init.LibraryName);
            writer.WriteLine("Hash: " + init.InitialHash);
        }

    }
}
