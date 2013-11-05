using System;
using System.IO;
using System.Threading;

namespace GitViz.Tests
{
    class TemporaryFolder : IDisposable
    {
        readonly string _path;

        public TemporaryFolder()
        {
            _path = System.IO.Path.Combine(System.IO.Path.GetTempPath(), "test" + DateTimeOffset.UtcNow.Ticks);
            Directory.CreateDirectory(_path);
        }

        public string Path
        {
            get { return _path; }
        }

        public void Dispose()
        {
            var triesRemaining = 25;
            while (triesRemaining > 0)
            {
                try
                {
                    Directory.Delete(_path, true);
                    return;
                }
                catch
                {
                    triesRemaining --;
                    Thread.SpinWait(100);
                }
            }
        }
    }
}
