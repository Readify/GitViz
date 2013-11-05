using System;
using System.Collections.Generic;
using System.Linq;

namespace GitViz.Logic
{
    public class LogRetriever
    {
        readonly GitCommandExecutor _executor;
        readonly LogParser _parser;

        public LogRetriever(
            GitCommandExecutor executor,
            LogParser parser = null)
        {
            _executor = executor;
            _parser = parser ?? new LogParser();
        }

        public IEnumerable<Commit> GetLog(int maxResults = 20)
        {
            var command = string.Format("log --all --pretty=format:\"{0}\" -{1}", _parser.ExpectedOutputFormat, maxResults);
            var log = _executor.ExecuteAndGetOutputStream(command);
            return _parser.ParseCommits(log);
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
