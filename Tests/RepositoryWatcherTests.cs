using System;
using System.Threading;
using GitViz.Logic;
using NUnit.Framework;

namespace GitViz.Tests
{
    [TestFixture]
    public class RepositoryWatcherTests
    {
        [Test]
        public void ShouldDetectChangeWhenCreatingNewBranchButNotSwitching()
        {
            Test(
                repo => { },
                repo => repo.RunCommand("branch foo"));
        }

        [Test]
        public void ShouldDetectChangeWhenCreatingNewTag()
        {
            Test(
                repo => { },
                repo => repo.RunCommand("tag foo"));
        }

        [Test]
        public void ShouldDetectChangeWhenCheckingOutADifferentHead()
        {
            Test(
                repo => repo.RunCommand("checkout -b foo"),
                repo => repo.RunCommand("checkout master"));
        }

        internal void Test(Action<TemporaryRepository> preSteps, Action<TemporaryRepository> triggerSteps)
        {
            using (var tempFolder = new TemporaryFolder())
            {
                var tempRepository = new TemporaryRepository(tempFolder);

                tempRepository.RunCommand("init");
                tempRepository.TouchFileAndCommit();

                preSteps(tempRepository);

                // Let everything stablize
                Thread.Sleep(RepositoryWatcher.DampeningIntervalInMilliseconds * 2);

                var triggered = false;
                var watcher = new RepositoryWatcher(tempFolder.Path, false);
                watcher.ChangeDetected += (sender, args) => { triggered = true; };

                triggerSteps(tempRepository);

                Thread.Sleep(RepositoryWatcher.DampeningIntervalInMilliseconds * 2);

                Assert.IsTrue(triggered);
            }
        }
    }
}
