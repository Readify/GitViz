using System;
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

        public string Execute(string command)
        {
            var process = CreateProcess(command);
            process.WaitForExit(10000);

            if (process.ExitCode == 0)
                return process.StandardOutput.ReadToEnd();

            var errorText = process.StandardError.ReadToEnd();
            throw new ApplicationException(errorText);
        }

        public StreamReader ExecuteAndGetOutputStream(string command)
        {
            var process = CreateProcess(command);
            return process.StandardOutput;
        }

        Process CreateProcess(string command)
        {
            var startInfo = new ProcessStartInfo
            {
                FileName = "git.exe",
                Arguments = command,
                WorkingDirectory = _repositoryPath,
                CreateNoWindow = true,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true
            };
            var process = Process.Start(startInfo);
            return process;
        }
    }
}
