using MediaDevices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace LLVC
{
    public class MediaPath
    {
        public MediaDevice Device { get; set; }
        public string Path { get; set; }
        public bool IsNormalPath => Device == null;

        public IEnumerable<string> EnumerateFiles()
        {
            if (IsNormalPath)
                return Directory.EnumerateFiles(Path);
            return Device.EnumerateFiles(Path);
        }
        public IEnumerable<string> EnumerateDirectories()
        {
            if (IsNormalPath)
                return Directory.EnumerateDirectories(Path);
            return Device.EnumerateDirectories(Path);
        }

        public bool IsDirectory()
        {
            if (IsNormalPath)
                return Directory.Exists(Path);
            return Device.DirectoryExists(Path);
        }
        public bool IsFile()
        {
            if (IsNormalPath)
                return File.Exists(Path);
            return Device.FileExists(Path);
        }

        public void CreateDirectory()
        {
            if (IsNormalPath)
                Directory.CreateDirectory(Path);
            Device.CreateDirectory(Path);
        }

        public long GetSize()
        {
            if (IsNormalPath)
                return new FileInfo(Path).Length;
            return (long)Device.GetFileInfo(Path).Length;
        }

        public DateTime GetLastWriteTime()
        {
            if (IsNormalPath)
                return new FileInfo(Path).LastWriteTime;
            var lwt = Device.GetFileInfo(Path).LastWriteTime;
            if (lwt.HasValue)
                return lwt.Value;
            return DateTime.Now;
        }

        public void Copy(MediaPath target, bool overwrite)
        {
            if (!IsFile())
                throw new Exception();
            if (!overwrite && target.IsFile())
                throw new Exception();

            if (IsNormalPath && target.IsNormalPath)
                File.Copy(Path, target.Path, overwrite);
            if (IsNormalPath && !target.IsNormalPath)
                using (Stream f = new FileStream(Path, FileMode.Open))
                    target.Device.UploadFile(f, target.Path);
            if (!IsNormalPath && target.IsNormalPath)
                using (Stream f = new FileStream(target.Path, FileMode.CreateNew))
                    Device.DownloadFile(Path, f);
            if(!IsNormalPath && !target.IsNormalPath)
                using (Stream f = new MemoryStream())
                {
                    Device.DownloadFile(Path, f);
                    Device.UploadFile(f, target.Path);
                }    
        }
    }
}
