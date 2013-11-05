using GitViz.Logic;
using NUnit.Framework;

namespace GitViz.Tests.LogParserTests
{
    [TestFixture]
    public class ParseCommit
    {
        [TestCase(
            "4be5ef1",
            Description = "Initial commit",
            Result = "{Hash:4be5ef1}")]
        [TestCase(
            "4e4224c 4be5ef1",
            Description = "Commit with one parent",
            Result = "{Hash:4e4224c,ParentHashes:[4be5ef1]}")]
        [TestCase(
            "d472fda 3c27924 5411a9f",
            Description = "Commit with two parents",
            Result = "{Hash:d472fda,ParentHashes:[3c27924,5411a9f]}")]
        [TestCase(
            "d472fda 3c27924 5411a9f 6789abc",
            Description = "Commit with three parents",
            Result = "{Hash:d472fda,ParentHashes:[3c27924,5411a9f,6789abc]}")]
        public string Test(string logLine)
        {
            var commit = LogParser.ParseCommit(logLine);
            return commit.ToJson();
        }
    }
}
