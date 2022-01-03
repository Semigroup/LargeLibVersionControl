using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.IO;

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
        /// copyTo [path]
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
                WriteStatusString();
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

                ParseCommand(line, words);
            }
        }

        public void WriteStatusString()
        {
            if (Controller == null)
                Console.Write("[null]: ");
            else
                Console.Write("[" + Controller.Protocol.LibraryName + "]: ");
        }

        public void ParseCommand(string line, List<string> words)
        {
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
            if (Controller == null)
            {
                Console.WriteLine("You need to select a library before you can use Get.");
                return;
            }

            long numberAllFiles = Controller.CountFiles(Controller.PathToLibrary);
            int top = Console.CursorTop;
            int left = Console.CursorLeft;
            long fileNumber = 0;
            string lastUpdateString = "";
            DateTime start = DateTime.Now;
            void statusUpdate(string relativeFilePath)
            {
                Console.SetCursorPosition(left, top);
                string newUpdateString = "Hashing file No. " + (fileNumber + 1) + " of " + numberAllFiles + ": " + relativeFilePath;
                Console.WriteLine(newUpdateString
                    + new string(' ', Math.Max(lastUpdateString.Length - newUpdateString.Length, 0)));
                lastUpdateString = newUpdateString;

                Console.SetCursorPosition(0, top + 3);
                WriteTimeEstimation(start, fileNumber, numberAllFiles);
                fileNumber++;
            }

            Diff diff = Controller.GetDiff(statusUpdate);
            ListDiff(diff);
            if (diff.IsEmpty)
                return;

            Console.WriteLine("Enter Commit to commit those changes:");
            WriteStatusString();
            string line = Console.ReadLine();
            var words = Split(line);

            if (words.Count != 1 && words[0].ToLower() != "commit")
            {
                ParseCommand(line, words);
                return;
            }

            Console.WriteLine("Enter a Title for the Commit:");
            string title = Console.ReadLine();

            Console.WriteLine("Enter a Message for the Commit:");
            string message = Console.ReadLine();

            Controller.Commit(title, message, DateTime.Now, diff);
        }
        public void WriteTimeEstimation(DateTime start, long currentItem, long numberAllItmes)
        {
            Console.WriteLine("Started at: " + start);
            Console.Write("[");
            int length = 100;
            for (int i = 0; i < length; i++)
                if ((i + 1) * numberAllItmes <= currentItem * length)
                    Console.Write('#');
                else
                    Console.Write(' ');
            Console.Write("]");
            Console.WriteLine(new string(' ', 10));

            TimeSpan past = DateTime.Now.Subtract(start);
            Console.Write("Running for: " + past);
            Console.WriteLine(new string(' ', 10));
            if (currentItem > 0)
                Console.Write("Estimated remaining time: "
                    + new TimeSpan((long)(past.Ticks * (numberAllItmes - currentItem) / currentItem)));
            Console.WriteLine(new string(' ', 10));
        }
        public void ListDiff(Diff diff)
        {
            List<FileEntry> deletions = new List<FileEntry>();
            List<FileEntry> changes = new List<FileEntry>();
            foreach (var update in diff.FileUpdates)
                switch (update.MyType)
                {
                    case FileUpdate.Type.Deletion:
                        deletions.Add(update.File);
                        break;
                    case FileUpdate.Type.Change:
                        changes.Add(update.File);
                        break;
                    default:
                        throw new NotImplementedException();
                }
            if (deletions.Count > 0)
            {
                Console.WriteLine("Deletions: " + deletions.Count);
                int i = 0;
                foreach (var file in deletions)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.Write(file.RelativePath + ", ");
                    Console.ForegroundColor = ConsoleColor.Gray;
                    Console.WriteLine(file.FileHash);
                    if (i++ > 99)
                    {
                        Console.WriteLine("and more ...");
                        break;
                    }
                }
                Console.WriteLine();
            }
            if (changes.Count > 0)
            {
                Console.WriteLine("Changes: " + changes.Count);
                int i = 0;
                foreach (var file in changes)
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.Write(file.RelativePath + ", ");
                    Console.ForegroundColor = ConsoleColor.Gray;
                    Console.WriteLine(file.FileHash);
                    if (i++ > 99)
                    {
                        Console.WriteLine("and more ...");
                        break;
                    }
                }
                Console.WriteLine();
            }
            Console.WriteLine(deletions.Count + " Deletions, " + changes.Count + " Changes");
        }
    }
}
