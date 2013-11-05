using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using GitViz.Logic.Annotations;

namespace GitViz.Logic
{
    public class ViewModel : INotifyPropertyChanged
    {
        string _repositoryPath;
        CommitGraph _graph = new CommitGraph();

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
                    var logRetriever = new LogRetriever(commandExecutor, _parser);

                    var commits = logRetriever.GetLog();
                    _graph = GenerateGraphFromCommits(commits);
                }
                else
                {
                    _graph = new CommitGraph();
                }

                OnPropertyChanged("Graph");
            }
        }

        private CommitGraph GenerateGraphFromCommits(IEnumerable<Commit> commits)
        {
            commits = commits.ToList();

            var graph = new CommitGraph();
            graph.AddVertexRange(commits);
            foreach (var commit in commits.Where(c => c.ParentHashes != null))
                foreach (var parentHash in commit.ParentHashes)
                {
                    var parent = commits.SingleOrDefault(c => c.Hash == parentHash);
                    if (parent != null) graph.AddEdge(new CommitEdge(commit, parent));
                }

            return graph;
        }

        public CommitGraph Graph
        {
            get { return _graph; }
        }

        static bool IsValidGitRepository(string path)
        {
            return Directory.Exists(path)
                && Directory.Exists(Path.Combine(path, ".git"));
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
