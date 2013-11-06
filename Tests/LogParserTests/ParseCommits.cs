using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using GitViz.Logic;
using NUnit.Framework;

namespace GitViz.Tests.LogParserTests
{
    [TestFixture]
    public class ParseCommits
    {
        static IEnumerable<Commit> Test(string input)
        {
            using (var stream = new MemoryStream(Encoding.ASCII.GetBytes(input)))
            using (var reader = new StreamReader(stream))
            {
                return new LogParser().ParseCommits(reader).ToArray();
            }
        }

        [Test]
        public void ShouldParseSingleCommitHash()
        {
            var results = Test(@"1383697102 4be5ef1");

            var expected = new[]
            {
                "{Hash:4be5ef1,CommitDate:1383697102,ShortHash:4be5ef1}"
            };

            CollectionAssert.AreEqual(
                expected,
                results.Select(c => c.ToJson()).ToArray());
        }
    }
}
