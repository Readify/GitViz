using System.Linq;
using GitViz.Logic;
using NUnit.Framework;

namespace GitViz.Tests
{
    [TestFixture]
    public class LogRetrieverTests
    {
        [Test]
        public void ShouldReturnSingleCommit()
        {
            using (var tempFolder = new TemporaryFolder())
            {
                var tempRepository = new TemporaryRepository(tempFolder);
                tempRepository.RunCommand("init");
                tempRepository.TouchFile("abc.txt");
                tempRepository.RunCommand("add -A");
                tempRepository.RunCommand("commit -m \"commit\"");

                var executor = new GitCommandExecutor(tempFolder.Path);

                var log = new LogRetriever(executor)
                    .GetLog()
                    .ToArray();

                Assert.AreEqual(1, log.Length);

                var commit = log.Single();
                Assert.IsNotNullOrEmpty(commit.Hash);
                Assert.AreEqual(7, commit.Hash.Length);
                Assert.IsNull(commit.ParentHashes);
            }
        }
    }
}
