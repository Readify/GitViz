using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using GitViz.Logic;
using NUnit.Framework;

namespace GitViz.Tests
{
    [TestFixture]
    public class FsckParserTests
    {
        static IEnumerable<string> Test(string input)
        {
            using (var stream = new MemoryStream(Encoding.ASCII.GetBytes(input)))
            using (var reader = new StreamReader(stream))
            {
                return new FsckParser().ParseUnreachableCommitsIds(reader).ToArray();
            }
        }

        [Test]
        public void ShouldParseSingleDanglingCommitHash()
        {
            var results = Test(@"unreachable commit 1928ce6245d999026679aa08353d0aa04b8bf4ca");
            var expected = new[] { "1928ce6245d999026679aa08353d0aa04b8bf4ca" };
            CollectionAssert.AreEqual(expected, results);
        }

        [Test]
        public void ShouldParseMultipleDanglingCommitHashes()
        {
            var results = Test(@"unreachable commit 1928ce6245d999026679aa08353d0aa04b8bf4ca
unreachable commit 5d021fe8cc958bc5ec945ec5066b394b7ffcfde8");

            var expected = new[]
            {
                "1928ce6245d999026679aa08353d0aa04b8bf4ca",
                "5d021fe8cc958bc5ec945ec5066b394b7ffcfde8"
            };

            CollectionAssert.AreEqual(expected, results);
        }

        [Test]
        public void ShouldIgnoreOtherLines()
        {
            var results = Test(@"foo
unreachable commit 1928ce6245d999026679aa08353d0aa04b8bf4ca
dangling tree 5d021fe8cc958bc5ec945ec5066b394b7ffcfde8
blah");
            var expected = new[] { "1928ce6245d999026679aa08353d0aa04b8bf4ca" };
            CollectionAssert.AreEqual(expected, results);
        }
    }
}
