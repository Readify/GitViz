using System;
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

        static readonly Regex ParseCommitRegex = new Regex(@"^(?<hash>\w{7})(?<parentHashes>( \w{7})+)?");

        internal static Commit ParseCommit(string logOutputLine)
        {
            var match = ParseCommitRegex.Match(logOutputLine.Trim());

            var parentHashes = match.Groups["parentHashes"].Success
                ? match.Groups["parentHashes"].Value.Split(new[] {' '}, StringSplitOptions.RemoveEmptyEntries)
                : null;

            return new Commit
            {
                Hash = match.Groups["hash"].Value,
                ParentHashes = parentHashes
            };
        }
    }
}
