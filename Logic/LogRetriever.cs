using System.Collections.Generic;

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

        public IEnumerable<Commit> GetLog()
        {
            var log = _executor.ExecuteAndGetOutputStream(string.Format("log --pretty=format:\"{0}\"", _parser.ExpectedOutputFormat));
            return _parser.ParseCommits(log);
        }
    }
}
