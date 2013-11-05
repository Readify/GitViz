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

        public IEnumerable<Commit> GetLog(int maxResults = 20)
        {
            var command = string.Format("log --pretty=format:\"{0}\" -{1}", _parser.ExpectedOutputFormat, maxResults);
            var log = _executor.ExecuteAndGetOutputStream(command);
            return _parser.ParseCommits(log);
        }
    }
}
