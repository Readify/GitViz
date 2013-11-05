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
            using (var repo = new TemporaryFolder())
            {
                var executor = new GitCommandExecutor(repo.Path);
                executor.Execute("init");

                var expectedGitFolderPath = Path.Combine(repo.Path, ".git");
                Assert.IsTrue(Directory.Exists(expectedGitFolderPath));
            }
        }

        [Test]
        public void ShouldThrowExceptionForFatalError()
        {
            using (var repo = new TemporaryFolder())
            {
                var executor = new GitCommandExecutor(repo.Path);
                Assert.Throws<ApplicationException>(() => executor.Execute("bad command"));
            }
        }
    }
}
