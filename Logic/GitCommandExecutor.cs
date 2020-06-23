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
            string errorText;
            try
            {
                var process = CreateProcess(command);
                process.WaitForExit(10000);

                if (process.ExitCode == 0)
                    return process.StandardOutput.ReadToEnd();
                errorText = process.StandardError.ReadToEnd();
            } catch (System.ComponentModel.Win32Exception)
            {
                errorText = "Could not locate git. Check it is installed and in your PATH settings";
            }

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
            Process process;
            process = Process.Start(startInfo);
            return process;
        }
    }
}
