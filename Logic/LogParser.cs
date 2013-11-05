using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace GitViz.Logic
{
    public class LogParser
    {
        public readonly string ExpectedOutputFormat = "%h %p %d";

        public IEnumerable<Commit> ParseCommits(StreamReader gitLogOutput)
        {
            while (!gitLogOutput.EndOfStream)
            {
                var line = gitLogOutput.ReadLine();
                if (string.IsNullOrWhiteSpace(line)) continue;
                yield return ParseCommit(line);
            }
            gitLogOutput.Close();
        }

        static readonly Regex ParseCommitRegex = new Regex(@"^(?<hash>\w{7})(?<parentHashes>( \w{7})+)?([ ]+\((?<refs>.*?)\))?");

        internal static Commit ParseCommit(string logOutputLine)
        {
            var match = ParseCommitRegex.Match(logOutputLine.Trim());

            var parentHashes = match.Groups["parentHashes"].Success
                ? match.Groups["parentHashes"].Value.Split(new[] {' '}, StringSplitOptions.RemoveEmptyEntries)
                : null;

            var refs = match.Groups["refs"].Success
                ? match.Groups["refs"]
                    .Value
                    .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(r => r.Trim())
                    .ToArray()
                : null;

            return new Commit
            {
                Hash = match.Groups["hash"].Value,
                ParentHashes = parentHashes,
                Refs = refs
            };
        }
    }
}
