using System;
using System.Collections.Generic;
using System.Linq;

namespace GitViz.Logic
{
    public class LogRetriever
    {
        readonly GitCommandExecutor _executor;
        readonly LogParser _logParser;
        readonly FsckParser _fsckParser;

        public LogRetriever(
            GitCommandExecutor executor,
            LogParser logParser = null,
            FsckParser fsckParser = null)
        {
            _executor = executor;
            _logParser = logParser ?? new LogParser();
            _fsckParser = fsckParser ?? new FsckParser();
        }

        public IEnumerable<Commit> GetRecentCommits(int maxResults = 20)
        {
            var command = string.Format("log --all --format=\"{0}\" -{1}", _logParser.ExpectedOutputFormat, maxResults);
            var log = _executor.ExecuteAndGetOutputStream(command);
            return _logParser.ParseCommits(log);
        }

        public IEnumerable<Commit> GetSpecificCommits(IEnumerable<string> hashes)
        {
            var command = string.Format("log --format=\"{0}\" --no-walk {1}", _logParser.ExpectedOutputFormat, string.Join(" ", hashes));
            var log = _executor.ExecuteAndGetOutputStream(command);
            return _logParser.ParseCommits(log);
        }

        public IEnumerable<string> GetRecentUnreachableCommitHashes(int maxResults = 20)
        {
            var fsck = _executor.ExecuteAndGetOutputStream("fsck --no-reflog --unreachable");
            var hashes = _fsckParser.ParseUnreachableCommitsIds(fsck);
            foreach (var hash in hashes.Take(maxResults))
            {
                yield return hash;
            }
            fsck.Close();
        }

        public string GetActiveReferenceName()
        {
            var branches = _executor.Execute("branch");
            return branches
                .Split(Environment.NewLine.ToCharArray())
                .Where(l => l.StartsWith("* "))
                .Select(l => l.Substring(2))
                .SingleOrDefault();
        }
    }
}
