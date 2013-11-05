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
        public string Test(string logLine)
        {
            var commit = LogParser.ParseCommit(logLine);
            return commit.ToJson();
        }
    }
}
