using System.Linq;
using GitViz.Logic;
using NUnit.Framework;

namespace GitViz.Tests
{
    [TestFixture]
    public class LogRetrieverTests
    {
        [Test]
        public void ShouldReturnSingleRecentCommit()
        {
            using (var tempFolder = new TemporaryFolder())
            {
                var tempRepository = new TemporaryRepository(tempFolder);
                tempRepository.RunCommand("init");
                tempRepository.TouchFileAndCommit();

                var executor = new GitCommandExecutor(tempFolder.Path);
                var log = new LogRetriever(executor).GetRecentCommits().ToArray();

                Assert.AreEqual(1, log.Length);
            }
        }

        [Test]
        public void ShouldReturnSingleRecentCommitWithHashButNoParents()
        {
            using (var tempFolder = new TemporaryFolder())
            {
                var tempRepository = new TemporaryRepository(tempFolder);
                tempRepository.RunCommand("init");
                tempRepository.TouchFileAndCommit();

                var executor = new GitCommandExecutor(tempFolder.Path);
                var log = new LogRetriever(executor).GetRecentCommits().ToArray();

                Assert.AreEqual(1, log.Length);

                var commit = log.Single();
                Assert.IsNotNullOrEmpty(commit.Hash);
                Assert.AreEqual(40, commit.Hash.Length);
                Assert.IsNull(commit.ParentHashes);
            }
        }

        [Test]
        public void ShouldReturnSingleRecentCommitWithLocalRefs()
        {
            using (var tempFolder = new TemporaryFolder())
            {
                var tempRepository = new TemporaryRepository(tempFolder);
                tempRepository.RunCommand("init");
                tempRepository.TouchFileAndCommit();

                var executor = new GitCommandExecutor(tempFolder.Path);
                var log = new LogRetriever(executor).GetRecentCommits().ToArray();

                var commit = log.Single();
                CollectionAssert.AreEqual(new[] { "HEAD", "master" }, commit.Refs);
            }
        }

        [Test]
        public void ShouldReturnTwoRecentCommits()
        {
            using (var tempFolder = new TemporaryFolder())
            {
                var tempRepository = new TemporaryRepository(tempFolder);
                tempRepository.RunCommand("init");
                tempRepository.TouchFileAndCommit();
                tempRepository.TouchFileAndCommit();

                var executor = new GitCommandExecutor(tempFolder.Path);
                var log = new LogRetriever(executor).GetRecentCommits().ToArray();

                Assert.AreEqual(2, log.Length);

                var commit = log.ElementAt(0);
                Assert.IsNotNullOrEmpty(commit.Hash);
                Assert.AreEqual(40, commit.Hash.Length);
                CollectionAssert.AreEqual(new[] { log.ElementAt(1).Hash }, commit.ParentHashes);

                commit = log.ElementAt(1);
                Assert.IsNotNullOrEmpty(commit.Hash);
                Assert.AreEqual(40, commit.Hash.Length);
                Assert.IsNull(commit.ParentHashes);
            }
        }

        [Test]
        public void ShouldReturnMostRecentCommitFirst()
        {
            using (var tempFolder = new TemporaryFolder())
            {
                var tempRepository = new TemporaryRepository(tempFolder);
                tempRepository.RunCommand("init");
                tempRepository.TouchFileAndCommit();
                tempRepository.TouchFileAndCommit();

                var executor = new GitCommandExecutor(tempFolder.Path);
                var log = new LogRetriever(executor).GetRecentCommits().ToArray();

                var firstCommitReturned = log.ElementAt(0);
                var secondCommitReturned = log.ElementAt(1);

                CollectionAssert.AreEqual(new[] { secondCommitReturned.Hash }, firstCommitReturned.ParentHashes);
            }
        }

        [Test]
        public void ShouldLimitNumberOfRecentCommitsRetrieved()
        {
            using (var tempFolder = new TemporaryFolder())
            {
                var tempRepository = new TemporaryRepository(tempFolder);
                tempRepository.RunCommand("init");
                for (var i = 0; i < 20; i ++)
                    tempRepository.TouchFileAndCommit();

                var executor = new GitCommandExecutor(tempFolder.Path);
                var log = new LogRetriever(executor).GetRecentCommits(10).ToArray();

                Assert.AreEqual(10, log.Length);
            }
        }

        [Test]
        public void ShouldReturnUnreachableCommitHashesAfterRewind()
        {
            using (var tempFolder = new TemporaryFolder())
            {
                var tempRepository = new TemporaryRepository(tempFolder);
                var executor = new GitCommandExecutor(tempFolder.Path);
                var log = new LogRetriever(executor);

                tempRepository.RunCommand("init");
                tempRepository.TouchFileAndCommit();
                var firstCommitHash = log.GetRecentCommits(1).Single().Hash;
                tempRepository.TouchFileAndCommit();
                var secondCommitHash = log.GetRecentCommits(1).Single().Hash;
                tempRepository.RunCommand("reset --hard " + firstCommitHash);

                var orphanedHashes = log.GetRecentUnreachableCommitHashes();
                CollectionAssert.AreEqual(new[] { secondCommitHash }, orphanedHashes);
            }
        }

        [Test]
        public void ShouldReturnMultipleUnreachableCommitHashesAfterRewind()
        {
            using (var tempFolder = new TemporaryFolder())
            {
                var tempRepository = new TemporaryRepository(tempFolder);
                var executor = new GitCommandExecutor(tempFolder.Path);
                var log = new LogRetriever(executor);

                tempRepository.RunCommand("init");
                tempRepository.TouchFileAndCommit();
                var firstCommitHash = log.GetRecentCommits(1).Single().Hash;

                for (var i = 0; i < 10; i++)
                    tempRepository.TouchFileAndCommit();

                tempRepository.RunCommand("reset --hard " + firstCommitHash);

                var orphanedHashes = log.GetRecentUnreachableCommitHashes(100);
                Assert.AreEqual(10, orphanedHashes.Count());
            }
        }

        [Test]
        public void ShouldLimitNumberOfUnreachableCommitHashesRetrieved()
        {
            using (var tempFolder = new TemporaryFolder())
            {
                var tempRepository = new TemporaryRepository(tempFolder);
                var executor = new GitCommandExecutor(tempFolder.Path);
                var log = new LogRetriever(executor);

                tempRepository.RunCommand("init");
                tempRepository.TouchFileAndCommit();
                var firstCommitHash = log.GetRecentCommits(1).Single().Hash;

                for (var i = 0; i < 20; i ++)
                    tempRepository.TouchFileAndCommit();

                tempRepository.RunCommand("reset --hard " + firstCommitHash);

                var orphanedHashes = log.GetRecentUnreachableCommitHashes(10);
                Assert.AreEqual(10, orphanedHashes.Count());
            }
        }

        [Test]
        public void ShouldReturnSameDataForSpecificCommitsAsWhenRetrievedViaRecentCommits()
        {
            using (var tempFolder = new TemporaryFolder())
            {
                var tempRepository = new TemporaryRepository(tempFolder);
                tempRepository.RunCommand("init");
                tempRepository.TouchFileAndCommit();
                tempRepository.TouchFileAndCommit();

                var executor = new GitCommandExecutor(tempFolder.Path);
                var logRetriever = new LogRetriever(executor);

                var recentCommits = logRetriever.GetRecentCommits().ToArray();
                var recentCommitHashes = recentCommits.Select(c => c.Hash);

                var specificCommits = logRetriever.GetSpecificCommits(recentCommitHashes);

                CollectionAssert.AreEqual(recentCommits, specificCommits, new SerializedObjectComparer());
            }
        }
    }
}
