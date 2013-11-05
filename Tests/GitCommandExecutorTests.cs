using System;
using System.IO;
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
            var temporaryRepositoryPath = Path.Combine(Path.GetTempPath(), "repo" + DateTimeOffset.UtcNow.Ticks);
            try
            {
                Directory.CreateDirectory(temporaryRepositoryPath);
                var executor = new GitCommandExecutor(temporaryRepositoryPath);
                executor.Execute("init");
            }
            finally
            {
                Directory.Delete(temporaryRepositoryPath, true);
            }
        }
    }
}
