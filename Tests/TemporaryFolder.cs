using System;
using System.IO;

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
            Directory.Delete(_path, true);
        }
    }
}
