using System.IO;
using GitViz.Logic;
using Newtonsoft.Json;
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
            Result = "{Hash:4e4224c,ParentHash:4be5ef1}")]
        public string Test(string logLine)
        {
            var commit = LogParser.ParseCommit(logLine);
            return commit.ToJson();
        }
    }
}
