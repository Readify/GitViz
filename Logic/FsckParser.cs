using System.Collections.Generic;
using System.IO;

namespace GitViz.Logic
{
    public class FsckParser
    {
        public IEnumerable<string> ParseDanglingCommitsIds(StreamReader fsck)
        {
            const string prefix = "dangling commit ";
            while (!fsck.EndOfStream)
            {
                var line = fsck.ReadLine();
                if (line == null || !line.StartsWith(prefix)) continue;
                yield return line.Substring(prefix.Length);
            }
            fsck.Close();
        }
    }
}