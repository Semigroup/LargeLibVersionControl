using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Diagnostics;


namespace LLVC
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Starting LargeLibraryVersionControl...");

            Assembly assembly = Assembly.GetExecutingAssembly();
            FileVersionInfo fileVersionInfo = FileVersionInfo.GetVersionInfo(assembly.Location);
            string version = fileVersionInfo.ProductVersion;
            Console.WriteLine("Version: " + version);

            var tool = new CmdLineTool();
            tool.Run();
        }
    }
}
