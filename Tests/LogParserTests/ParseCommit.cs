using GitViz.Logic;
using NUnit.Framework;

namespace GitViz.Tests.LogParserTests
{
    [TestFixture]
    public class ParseCommit
    {
        [TestCase(
            "1383697102 4be5ef1",
            Description = "Initial commit",
            ExpectedResult = "{Hash:4be5ef1,CommitDate:1383697102,ShortHash:4be5ef1}")]
        [TestCase(
            "1383697102 4be5ef1 (HEAD, master)",
            Description = "Initial commit with head and master",
            ExpectedResult = "{Hash:4be5ef1,Refs:[HEAD,master],CommitDate:1383697102,ShortHash:4be5ef1}")]
        [TestCase(
            "1383697102 4be5ef1 (HEAD, origin/master, origin/HEAD, master)",
            Description = "Initial commit with head and remote master",
            ExpectedResult = "{Hash:4be5ef1,Refs:[HEAD,origin/master,origin/HEAD,master],CommitDate:1383697102,ShortHash:4be5ef1}")]
        [TestCase(
            "1383697102 4e4224c 4be5ef1",
            Description = "Commit with one parent",
            ExpectedResult = "{Hash:4e4224c,ParentHashes:[4be5ef1],CommitDate:1383697102,ShortHash:4e4224c}")]
        [TestCase(
            "1383697102 d472fda 3c27924 5411a9f",
            Description = "Commit with two parents",
            ExpectedResult = "{Hash:d472fda,ParentHashes:[3c27924,5411a9f],CommitDate:1383697102,ShortHash:d472fda}")]
        [TestCase(
            "1383697102 d472fda 3c27924 5411a9f 6789abc",
            Description = "Commit with three parents",
            ExpectedResult = "{Hash:d472fda,ParentHashes:[3c27924,5411a9f,6789abc],CommitDate:1383697102,ShortHash:d472fda}")]
        public string Test(string logLine)
        {
            var commit = LogParser.ParseCommit(logLine);
            return commit.ToJson();
        }
    }
}
