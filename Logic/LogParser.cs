using System.Collections.Generic;
using System.IO;

namespace GitViz.Logic
{
    public class LogParser
    {
        public IEnumerable<Commit> ParseCommits(StreamReader gitLogOutput)
        {
            while (!gitLogOutput.EndOfStream)
            {
                var line = gitLogOutput.ReadLine();
                if (string.IsNullOrWhiteSpace(line)) continue;
                yield return ParseCommit(line);
            }
        }

        internal static Commit ParseCommit(string logOutputLine)
        {
            var hash = logOutputLine.Substring(0, 7);
            return new Commit {Hash = hash};
        }
    }
}
