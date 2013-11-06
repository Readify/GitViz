using System;
using System.IO;
using System.Threading;

namespace GitViz.Logic
{
    public class RepositoryWatcher : IDisposable
    {
        public const int DampeningIntervalInMilliseconds = 100;

        readonly FileSystemWatcher _watcher;

        public event EventHandler ChangeDetected;

        protected virtual void OnChangeDetected()
        {
            var handler = ChangeDetected;
            if (handler != null) handler(this, EventArgs.Empty);
        }

        public RepositoryWatcher(string path)
        {
            var gitFolder = Path.Combine(path, @".git");
            _watcher = new FileSystemWatcher(gitFolder)
            {
                EnableRaisingEvents = true,
                IncludeSubdirectories = true
            };

            var lag = new Timer(state => OnChangeDetected(), null, Timeout.Infinite, Timeout.Infinite);
            FileSystemEventHandler onChanged = (sender, args) => lag.Change(TimeSpan.FromMilliseconds(DampeningIntervalInMilliseconds), Timeout.InfiniteTimeSpan);

            _watcher.Changed += onChanged;
            _watcher.Created += onChanged;
            _watcher.Deleted += onChanged;
        }

        public void Dispose()
        {
            _watcher.Dispose();
        }

        ~RepositoryWatcher()
        {
            Dispose();
        }
    }
}
