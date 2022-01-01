using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace LLVC
{
    public class LibraryController
    {
        public string PathToLibrary { get; private set; }
        public string PathToLLVC { get; private set; }

        public Init Init { get; private set; }
        public IList<Commit> Commits { get; private set; }
        public Index DeFactoIndex { get; private set; }
        public Index DeIureIndex { get; private set; }

        public LibraryController(string PathToLibrary)
        {
            this.PathToLibrary = PathToLibrary;
            if (!Directory.Exists(PathToLibrary))
                throw new DirectoryNotFoundException(PathToLibrary + " does not exist!");

            this.PathToLLVC = Path.Combine(PathToLibrary, "/.llvc/");
            if (!Directory.Exists(PathToLLVC))
                throw new DirectoryNotFoundException(PathToLLVC + " does not exist!");

            string initFile = Path.Combine(PathToLLVC, ".init");
            if (!File.Exists(initFile))
                throw new FileNotFoundException(initFile + " does not exist!");

            XmlSerializer initSerializer = new XmlSerializer(typeof(Init));
            Init = (Init)initSerializer.Deserialize(File.OpenRead(initFile));

            XmlSerializer commitSerializer = new XmlSerializer(typeof(Commit));
            List<Commit> commits = new List<Commit>();
            foreach (string commitFile in Directory.EnumerateFiles(PathToLLVC, "*.commit"))
                commits.Add((Commit)commitSerializer.Deserialize(File.OpenRead(commitFile)));

            //Broken Hash Chain überprüfen
        }

    }
}