using System.Diagnostics;
using System.IO;

namespace GitViz.Logic
{
    public class GitCommandExecutor
    {
        readonly string _repositoryPath;

        public GitCommandExecutor(string repositoryPath)
        {
            _repositoryPath = repositoryPath;
        }

        public StreamReader Execute(string command)
        {
            var startInfo = new ProcessStartInfo
            {
                FileName = "git.exe",
                Arguments = command,
                WorkingDirectory = _repositoryPath,
                CreateNoWindow = false,
                WindowStyle = ProcessWindowStyle.Hidden,
                UseShellExecute = false,
                RedirectStandardOutput = true
            };
            var process = Process.Start(startInfo);
            return process.StandardOutput;
        }
    }
}
