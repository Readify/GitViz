using System;
using System.Globalization;
using System.IO;
using GitViz.Logic;

namespace GitViz.Tests
{
    class TemporaryRepository
    {
        readonly string _folderPath;
        readonly GitCommandExecutor _executor;

        public TemporaryRepository(TemporaryFolder folder)
        {
            _folderPath = folder.Path;
            _executor = new GitCommandExecutor(folder.Path);
        }

        public string RunCommand(string command)
        {
            return _executor.Execute(command);
        }

        public void TouchFileAndCommit()
        {
            var filePath = Path.Combine(_folderPath, "abc.txt");
            File.WriteAllText(filePath, DateTimeOffset.UtcNow.Ticks.ToString(CultureInfo.InvariantCulture));

            RunCommand("add -A");
            RunCommand("commit -m \"commit\"");
        }
    }
}
