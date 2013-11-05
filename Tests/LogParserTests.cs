using System.Linq;
using GitViz.Logic;
using NUnit.Framework;

namespace GitViz.Tests
{
    [TestFixture]
    public class LogParserTests
    {
        [Test]
        public void ParseSingleCommitHash()
        {
            const string input = @"4be5ef1";

            var result = new LogParser()
                .ParseCommits(input)
                .ToArray();

            Assert.AreEqual(1, result.Length);
            Assert.AreEqual("4be5ef1", result[0].Hash);
        }
    }
}
