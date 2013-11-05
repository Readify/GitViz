using System.Collections.ObjectModel;
using System.IO;
using System.Linq;

namespace GitViz.Logic
{
    public class ViewModel
    {
        string _repositoryPath;
        LogRetriever _logRetriever;

        readonly LogParser _parser = new LogParser();

        public ViewModel()
        {
            RepositoryPath = @"c:\temp\git-bash-test";
        }

        public string RepositoryPath
        {
            get { return _repositoryPath; }
            set
            {
                _repositoryPath = value;
                if (IsValidGitRepository(_repositoryPath))
                {
                    var commandExecutor = new GitCommandExecutor(_repositoryPath);
                    _logRetriever = new LogRetriever(commandExecutor, _parser);

                    _commits.Clear();
                    var commits =_logRetriever.GetLog().ToList();
                    commits.ForEach(_commits.Add);
                }
                else
                {
                    _logRetriever = null;
                    _commits.Clear();
                }
            }
        }

        readonly ObservableCollection<Commit> _commits = new ObservableCollection<Commit>();
        public ObservableCollection<Commit> Commits
        {
            get { return _commits; }
        }

        static bool IsValidGitRepository(string path)
        {
            return Directory.Exists(path)
                && Directory.Exists(Path.Combine(path, ".git"));
        }
    }
}
