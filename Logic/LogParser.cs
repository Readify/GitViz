using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

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

        static readonly Regex ParseCommitRegex = new Regex(@"(?<hash>\w{7})( (?<parentHash>\w{7}))?");

        internal static Commit ParseCommit(string logOutputLine)
        {
            var match = ParseCommitRegex.Match(logOutputLine.Trim());
            return new Commit
            {
                Hash = match.Groups["hash"].Value,
                ParentHash = match.Groups["parentHash"].Success ? match.Groups["parentHash"].Value : null
            };
        }
    }
}
