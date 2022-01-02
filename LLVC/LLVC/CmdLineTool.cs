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
        /// get : gibt aktuelle Änderungen an, danach kann commit ausgelöst werden
        /// commit : commited changes und fragt nach titel und message (nur echte changes können committed werden)
        /// 
        /// compareTo [path]
        /// sync : nach compareTo, geht nur, wenn keine uncommitted changes auf dem remote sind
        /// forceSync: geht direkt nach file-indices
        /// 
        /// removeEmptyFolders
        /// replaceWhiteSpaces
        /// 
        /// diagnose [path] : falls nicht korrekt
        /// 
        /// </summary>
        public void Run()
        {
            while (true)
            {
                string line = Console.ReadLine();
                var words = Split(line);
                foreach (var word in words)
                    Console.WriteLine(word);


                //if (Controller == null)
                //    Console.Write("null: ");
                //else
                //    Console.Write(Controller.PathToLibrary + ": ");

                //string line = Console.ReadLine();

                //var words = line.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                //if (words.Length > 0)
                //    switch (words[0])
                //    {
                //        case "Init":
                //            if (words.Length != 2)
                //                Console.WriteLine("Init needs to be followed by a path.");
                //            break;
                //        default:
                //            break;
                //    }
            }
        }

        public List<string> Split(string line)
        {
            List<string> words = new List<string>();
            int start = 0;
            int length = 0;
            bool encapsulated = false;
            for (int i = 0; i < line.Length; i++)
            {
                switch (line[i])
                {
                    case ' ':
                        if (encapsulated)
                            length++;
                        else
                        {
                            if (length > 0)
                            {
                                words.Add(line.Substring(start, length));
                                length = 0;
                            }
                            start = i + 1;
                        }
                        break;
                    case '\"':
                        if (length > 0)
                        {
                            words.Add(line.Substring(start, length));
                            length = 0;
                        }
                        start = i + 1;

                        encapsulated = !encapsulated;
                        break;
                    default:
                        length++;
                        break;
                }
            }

            return words;
        }

        public void Select(string path)
        {
            try
            {
                Controller = new LibraryController(path);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Controller = null;
            }
        }
        public void Init(string path)
        {

        }
        public void Get()
        {

        }
    }
}
