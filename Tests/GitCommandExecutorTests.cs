using GitViz.Logic;
using NUnit.Framework;

namespace GitViz.Tests
{
    [TestFixture]
    public class GitCommandExecutorTests
    {
        [Test]
        public void ShouldGitInit()
        {
            using (var repo = new TemporaryFolder())
            {
                var executor = new GitCommandExecutor(repo.Path);
                executor.Execute("init").ReadToEnd();
            }
        }
    }
}
