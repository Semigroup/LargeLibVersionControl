using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using MediaDevices;

namespace LLVC
{
    public class CmdLineTool
    {
        public LibraryController Controller { get; set; }
        public HashFunction HashFunction { get; set; } = new HashFunction();
        public MediaController MediaController { get; set; } = new MediaController();

        public int Additions { get; set; } = -1;
        public int Changes { get; set; } = -1;
        public int Deletions { get; set; } = -1;

        public void ListMediaDevices()
        {
            var devices = MediaDevice.GetDevices();
            if (devices.Count() == 0)
            {
                Console.WriteLine("No devices found.");
                return;
            }
            foreach (var item in devices)
            {
                string name = item.FriendlyName + ", " + item.Manufacturer;
                if (item.IsConnected)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.Write(name);
                    Console.ForegroundColor = ConsoleColor.Gray;
                    Console.Write(": ");
                    Console.Write(item.Model);
                    Console.Write(", ");
                    Console.Write(item.FirmwareVersion);
                    Console.Write(", ");
                    Console.Write(item.Protocol);
                    Console.WriteLine();
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(name);
                    Console.ForegroundColor = ConsoleColor.Gray;
                }
            }
        }
        public MediaDevice GetDevice(string friendlyname)
        {
            var devices = MediaDevice.GetDevices();
            foreach (var item in devices)
                if (item.FriendlyName.ToLower().Contains(friendlyname.ToLower()))
                    return item;
            return null;
        }
        public void ConnectMediaDevice(string friendlyname, bool disconnect)
        {
            var device = GetDevice(friendlyname);
            if (device == null)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Couldnt find device with name '" + friendlyname + "'!");
                Console.ForegroundColor = ConsoleColor.Gray;
                return;
            }
            if (disconnect)
                MediaController.DisconnectFromDevice(device);
            else
                MediaController.ConnectToDevice(device);
        }


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
        /// sortMusic : erstellt neue Library und verschiebt musikdateien in Album-Künstler/Album/Titel
        /// 
        /// </summary>
        public void Run()
        {
            //FindPhone();
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
            {
                Console.Write("[" + Controller.Protocol.LibraryName);
                Console.Write(" ");
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write(Additions >= 0 ? Additions.ToString() : "?");
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.Write(", ");

                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write(Changes >= 0 ? Changes.ToString() : "?");
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.Write(", ");

                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write(Deletions >= 0 ? Deletions.ToString() : "?");
                Console.ForegroundColor = ConsoleColor.Gray;

                Console.Write("]: ");

            }
        }
        public void WritePromptLine(string prompt)
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(prompt);
            Console.ForegroundColor = ConsoleColor.Gray;
        }
        public void PrintHelp()
        {
            Console.WriteLine();

            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("Select [path]");
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine("Select will open the library at the given path, if possible.");
            Console.WriteLine();

            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("Init [path]");
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine("Init will create a new library at the given path.");
            Console.WriteLine();

            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("Get [full?]");
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine("Get will make a quick check if there have been any file changes.");
            Console.WriteLine("Get full will make a full check if there have been any changes by");
            Console.WriteLine("computing SHA256 values of all files in the current library.");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("Commit");
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine("After calling Get, you can enter Commit to commit the detected file changes.");
            Console.WriteLine();

            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("CopyTo [path]");
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine("CopyTo will copy the whole content of the current library to the given path.");
            Console.WriteLine("Existing files will be overwritten in the process!");
            Console.WriteLine();

            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("CompareTo [path]");
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine("CompareTo will compare the state of the current library " +
                "with the state of a library at the given path.");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("Sync");
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine("After calling CompareTo, if one of the given libraries is ahead by a number of commits,");
            Console.WriteLine("you can enter Sync to update the library that is behind.");
            Console.WriteLine();
        }

        public void ParseCommand(string line, List<string> words)
        {
            switch (words[0].ToLower())
            {
                case "disconnect":
                case "connect":
                    if (words.Count != 2)
                        Console.WriteLine("connect needs to be followed by a device name.");
                    else
                        ConnectMediaDevice(words[1], words[0].ToLower() == "disconnect");
                    break;

                case "listdevices":
                    ListMediaDevices();
                    break;

                case "help":
                    PrintHelp();
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
                    if (words.Count == 1)
                        Get(false);
                    else if (words.Count == 2)
                    {
                        if (words[1].ToLower() == "full")
                            Get(true);
                        else
                            Console.WriteLine("Get can only be followed by 'full'.");
                    }
                    else
                        Console.WriteLine("Get can only be followed by 'full'.");
                    break;

                case "copyto":
                    if (words.Count != 2)
                        Console.WriteLine("CopyTo needs to be followed by a path.");
                    else
                        CopyTo(words[1]);
                    break;

                case "compareto":
                    if (words.Count != 2)
                        Console.WriteLine("CompareTo needs to be followed by a path.");
                    else
                        CompareTo(words[1]);
                    break;

                default:
                    Console.WriteLine("Couldnt parse " + line);
                    Console.WriteLine("Enter 'help' to list commands.");
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
            this.Additions = this.Deletions = this.Changes = -1;
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
            WritePromptLine("Enter a name for the new Library:");
            var name = Console.ReadLine();

            this.Additions = this.Deletions = this.Changes = -1;

            try
            {
                Controller = LibraryController.Create(path, name, BitConverter.GetBytes(DateTime.Now.Ticks));
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Controller = null;
            }
        }
        public void Get(bool fullCheck)
        {
            if (Controller == null)
            {
                Console.WriteLine("You need to select a library before you can use Get.");
                return;
            }

            Diff diff = fullCheck ? Controller.GetFullDiff() : Controller.GetQuickDiff();
            ListDiff(diff);

            if (diff.IsEmpty)
                return;

            WritePromptLine("Enter Commit to commit those changes:");
            WriteStatusString();
            string line = Console.ReadLine();
            var words = Split(line);

            if (words.Count != 1 || words[0].ToLower() != "commit")
            {
                ParseCommand(line, words);
                return;
            }

            WritePromptLine("Enter a Title for the Commit:");
            string title = Console.ReadLine();

            WritePromptLine("Enter a Message for the Commit:");
            string message = Console.ReadLine();

            Controller.Commit(title, message, DateTime.Now, diff);
            this.Additions = this.Changes = this.Deletions = 0;
        }
        public void CopyTo(string newLibraryPath)
        {
            if (Controller == null)
            {
                Console.WriteLine("You need to select a library before you can use CopyTo.");
                return;
            }
            Diff diff = Controller.GetQuickDiff();
            if (!diff.IsEmpty)
            {
                Console.WriteLine("There are uncommitted changes.");
                Console.WriteLine("Commit those changes before calling CopyTo.");
                return;
            }

            if (!Directory.Exists(newLibraryPath))
            {
                Console.WriteLine("The directory " + newLibraryPath + "does not exist.");
                WritePromptLine("Enter Yes, if " + newLibraryPath + " shall be created.");
                if (!GetYes())
                {
                    Console.WriteLine("CopyTo Canceled.");
                    return;
                }
                try
                {
                    Directory.CreateDirectory(newLibraryPath);
                }
                catch (Exception e)
                {
                    Console.WriteLine("Creation of " + newLibraryPath + " failed:");
                    Console.WriteLine(e);
                    return;
                }
                Console.WriteLine("Succesfully created " + newLibraryPath + ".");
            }

            WritePromptLine("Enter name for new library:");
            string newLibraryName = Console.ReadLine();

            Console.WriteLine("Started copying of Files...");
            Copier.CopyTo(Controller, newLibraryPath, newLibraryName);

            Console.WriteLine("Finished copying of Files...");

        }
        public void CompareTo(string newLibraryPath)
        {
            if (Controller == null)
            {
                Console.WriteLine("You need to select a library before you can use CompareTo.");
                return;
            }
            Diff diff = Controller.GetQuickDiff();
            if (!diff.IsEmpty)
            {
                Console.WriteLine("There are uncommitted changes in " + Controller.FullName);
                Console.WriteLine("Commit those changes before calling CompareTo.");
                return;
            }

            LibraryController newController = null;
            try
            {
                newController = new LibraryController(newLibraryPath);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return;
            }
            diff = newController.GetQuickDiff();
            if (!diff.IsEmpty)
            {
                Console.WriteLine("There are uncommitted changes in " + newController.FullName);
                Console.WriteLine("Commit those changes before calling CompareTo.");
                return;
            }

            var comparison = new ComparisonResult(Controller.Protocol, newController.Protocol);

            switch (comparison.Type)
            {
                case ComparisonResult.ResultType.Synchronous:
                    Console.WriteLine(Controller.FullName + " and " + newController.FullName + " are synchronous.");
                    break;
                case ComparisonResult.ResultType.AIsAhead:
                    Console.WriteLine(Controller.FullName + " is ahead by " + comparison.HeadStart + " commits.");
                    Synchronize(Controller, newController);
                    break;
                case ComparisonResult.ResultType.BIsAhead:
                    Console.WriteLine(newController.FullName + " is ahead by " + comparison.HeadStart + " commits.");
                    Synchronize(newController, Controller);
                    break;
                case ComparisonResult.ResultType.NotComparable:
                    Console.WriteLine(Controller.FullName + " and " + newController.FullName + " cannot be compared. " +
                        "They have different hash seeds.");
                    break;
                case ComparisonResult.ResultType.Dismerged:
                    Console.WriteLine(Controller.FullName + " and " + newController.FullName + " dismerged at commit No."
                        + comparison.DismergedAt + ".");
                    break;
                default:
                    throw new NotImplementedException();
            }
        }
        public void Synchronize(LibraryController controllerA, LibraryController controllerB)
        {
            WritePromptLine("Enter sync to synchronize " + controllerB.FullName + ":");
            string line = Console.ReadLine();
            var words = Split(line);
            if (words.Count != 1 || words[0].ToLower() != "sync")
            {
                ParseCommand(line, words);
                return;
            }
            int bCommits = controllerB.Protocol.Commits.Count;
            var commitsAhead = controllerA.Protocol.Commits.Skip(bCommits);

            var toApply = Diff.Accumulate(
                commitsAhead.Select(c => c.Diff)
                    );

            foreach (var update in toApply.Values)
                if (update.MyType == FileUpdate.Type.Deletion)
                    File.Delete(Path.Combine(controllerB.PathToLibrary, update.File.RelativePath));

            Copier.CopyTo(
                controllerA.PathToLibrary,
                controllerB.PathToLibrary,
                toApply.Values.Where(x => x.MyType == FileUpdate.Type.Change).Select(x => x.File)
                );

            controllerB.Protocol.Commits.AddRange(commitsAhead);
            foreach (var item in toApply.Values)
                if (item.MyType == FileUpdate.Type.Deletion)
                    controllerB.LookUp.Table.Remove(item.File.RelativePath);
                else
                    controllerB.LookUp.Table[item.File.RelativePath]
                        = controllerA.LookUp.Table[item.File.RelativePath].Clone() as FileEntry;
            controllerB.SaveProtocol();
            controllerB.SaveLookUpTable();
        }

        public bool GetYes()
        {
            string answer = Console.ReadLine().Trim().ToLower();
            return answer == "yes" || answer == "y";
        }
        public static void WriteTimeEstimation(DateTime start, long currentItem, long numberAllItmes)
        {
            char[] waitSymbols = { '|', '/', '-', '\\' };

            Console.WriteLine("Started at: " + start);
            Console.Write("[");
            int length = 100;
            for (int i = 0; i < length; i++)
                if ((i + 1) * numberAllItmes <= currentItem * length)
                    Console.Write('#');
                else if (i * numberAllItmes <= currentItem * length)
                    Console.Write(waitSymbols[currentItem % waitSymbols.Length]);
                else
                    Console.Write(' ');
            Console.Write("]");
            Console.WriteLine(new string(' ', 10));

            TimeSpan past = DateTime.Now.Subtract(start);
            Console.Write("Running for: " + past);
            Console.WriteLine(new string(' ', 10));
            if (currentItem > 0)
                Console.Write("Estimated remaining time: "
                    + new TimeSpan((long)(past.Ticks * 1.0 * (numberAllItmes - currentItem) / currentItem)));
            Console.WriteLine(new string(' ', 10));
        }

        private void ListFilesEntries(List<FileEntry> entries, string operationName, ConsoleColor operationColor)
        {
            if (entries.Count > 0)
            {
                Console.WriteLine(operationName + ": " + entries.Count);
                int i = 0;
                foreach (var file in entries)
                {
                    Console.ForegroundColor = operationColor;
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
        }

        public void ListDiff(Diff diff)
        {
            (var additions, var changes, var deletions) = diff.SortUpdates();
            this.Deletions = deletions.Count;
            this.Additions = additions.Count;
            this.Changes = changes.Count;

            ListFilesEntries(deletions, "Deletions", ConsoleColor.Red);
            ListFilesEntries(changes, "Changes", ConsoleColor.Yellow);
            ListFilesEntries(additions, "Additions", ConsoleColor.Green);
            Console.WriteLine(deletions.Count + " Deletions, " + changes.Count + " Changes, " + additions.Count + " Additions");
        }
        public static string GetByteDescription(long bytes)
        {
            string[] sizes = { "B", "KB", "MB", "GB", "TB" };
            for (int i = sizes.Length - 1; i >= 0; i--)
            {
                long size = 1L << (i * 10);
                if (bytes >= size)
                    return (bytes * 1.0 / size).ToString("G3") + " " + sizes[i];
            }
            return bytes + " B";
        }
        public static void WorkOnFiles(IEnumerable<FileEntry> files, string work, Action<FileEntry> workAction)
        {
            long totalNumber = 0;
            long totalSize = 0;
            foreach (var item in files)
            {
                totalNumber++;
                totalSize += item.Size;
            }

            bool printProgress = totalSize >= 1 << 27;

            int left = Console.CursorLeft;
            int top = Console.CursorTop;
            string lastUpdateLine1 = "";
            string lastUpdateLine2 = "";
            string lastUpdateLine3 = "";
            DateTime start = DateTime.Now;
            long currentSize = 0;
            long fileNumber = 0;

            foreach (var entry in files)
            {
                if (printProgress)
                {
                    TimeSpan time = DateTime.Now.Subtract(start);

                    string newUpdateLine1 =
                    work + "ing file No. " + (fileNumber + 1) + " of "
                    + totalNumber + ": " + entry.RelativePath;
                    string newUpdateLine2 =
                        work + "ed " + CmdLineTool.GetByteDescription(currentSize)
                        + " Bytes of " + CmdLineTool.GetByteDescription(totalSize);
                    string newUpdateLine3 =
                       "Average speed: " +
                       CmdLineTool.GetByteDescription((long)(currentSize / time.TotalSeconds)) + " per sec.";

                    Console.SetCursorPosition(left, top);
                    Console.WriteLine(newUpdateLine1
                        + new string(' ', Math.Max(lastUpdateLine1.Length - newUpdateLine1.Length, 0)));
                    lastUpdateLine1 = newUpdateLine1;

                    Console.SetCursorPosition(left, top + 2);
                    Console.WriteLine(newUpdateLine2
                        + new string(' ', Math.Max(lastUpdateLine2.Length - newUpdateLine2.Length, 0)));
                    lastUpdateLine2 = newUpdateLine2;

                    Console.SetCursorPosition(left, top + 3);
                    Console.WriteLine(newUpdateLine3
                        + new string(' ', Math.Max(lastUpdateLine3.Length - newUpdateLine3.Length, 0)));
                    lastUpdateLine3 = newUpdateLine3;

                    Console.SetCursorPosition(left, top + 5);
                    CmdLineTool.WriteTimeEstimation(start, currentSize, totalSize);
                    fileNumber++;
                    currentSize += entry.Size;
                }
                workAction(entry);
            }
        }
    }
}
