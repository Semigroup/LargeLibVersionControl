using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;

namespace LLVC
{
    public class CmdLineTool
    {
        public LibraryController Controller { get; set; }
        public SHA256 SHA256 { get; set; } = SHA256.Create();

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
                if (Controller == null)
                    Console.Write("[null]: ");
                else
                    Console.Write("[" + Controller.Protocol.LibraryName + "]: ");

                string line = Console.ReadLine();
                List<string> words = null;
                try
                {
                    words = Split(line);
                }
                catch (ArgumentException)
                {
                    Console.WriteLine("Couldnt parse " + line);
                    Console.WriteLine("You need to close the quotation marks!");
                    continue;
                }

                if (words.Count == 0)
                    continue;

                switch (words[0].ToLower())
                {
                    case "help":
                        Console.WriteLine("ToDo");
                        break;

                    case "init":
                        if (words.Count != 2)
                            Console.WriteLine("Init needs to be followed by a path.");
                        else
                            Init(words[1]);
                        break;

                    case "select":
                        if (words.Count != 2)
                            Console.WriteLine("Select needs to be followed by a path.");
                        else
                            Select(words[1]);
                        break;

                    case "get":
                        if (words.Count != 1)
                            Console.WriteLine("Get may not have any arguments.");
                        else
                            Get();
                        break;

                    default:
                        Console.WriteLine("Couldnt parse " + line);
                        break;
                }
            }
        }

        public List<string> Split(string line)
        {
            List<string> words = new List<string>();
            int start = 0;
            int length = 0;
            bool encapsulated = false;

            void addCurrent()
            {
                if (length > 0 && start < line.Length)
                {
                    words.Add(line.Substring(start, length));
                    length = 0;
                }
            }

            for (int i = 0; i < line.Length; i++)
                switch (line[i])
                {
                    case ' ':
                        if (encapsulated)
                            length++;
                        else
                        {
                            addCurrent();
                            start = i + 1;
                        }
                        break;
                    case '\"':
                        addCurrent();
                        start = i + 1;

                        encapsulated = !encapsulated;
                        break;
                    default:
                        length++;
                        break;
                }
            addCurrent();

            if (encapsulated)
                throw new ArgumentException("Quote Marks not closed!");

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
            Console.WriteLine("Creating a new library at " + path + ".");
            Console.WriteLine("Enter a name for the Library:");
            var name = Console.ReadLine();
            var initialHash = new HashValue(SHA256.ComputeHash(BitConverter.GetBytes(DateTime.Now.Ticks)));

            try
            {
                LibraryController.Create(path, name, initialHash);
                Controller = new LibraryController(path);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Controller = null;
            }
        }
        public void Get()
        {

        }
    }
}
