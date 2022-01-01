using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LLVC
{
    public class LibraryController
    {
        public string PathToLibrary { get; private set; }

        public Init Init { get; set; }
        public IList<Commit> Commits { get; set; }

        public LibraryController(string PathToLibrary)
        {
            this.PathToLibrary = PathToLibrary;
        }
    }
}
