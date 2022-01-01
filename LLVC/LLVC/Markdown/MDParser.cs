using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LLVC.Markdown
{
   public class MDParser
    {
        public IEnumerable<string> RemoveComments(string text)
        {
            string[] lines = text.Split('\n', '\r');
            for (int i = 0; i < lines.Length; i++)
            {
                int p = lines[i].IndexOf("//");
                lines[i] = lines[i].Substring(0, p).Trim();
            }
            return lines.Where(line => line.Length > 0);
        }
    }
}
