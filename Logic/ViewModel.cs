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
        FileSystemWatcher _watcher;

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

                    _watcher = new FileSystemWatcher(Path.Combine(_repositoryPath, @".git\refs"))
                    {
                        EnableRaisingEvents = true,
                        IncludeSubdirectories = true
                    };
                    FileSystemEventHandler onChanged = (sender, args) =>
                    {
                        commits = logRetriever.GetLog();
                        _graph = GenerateGraphFromCommits(commits);
                        OnPropertyChanged("Graph");
                    };
                    _watcher.Changed += onChanged;
                    _watcher.Created += onChanged;
                    _watcher.Deleted += onChanged;
                }
                else
                {
                    _graph = new CommitGraph();
                    if (_watcher != null) _watcher.Dispose();
                }

                OnPropertyChanged("Graph");
            }
        }

        private CommitGraph GenerateGraphFromCommits(IEnumerable<Commit> commits)
        {
            commits = commits.ToList();

            var graph = new CommitGraph();

            var commitVertices = commits.Select(c => new Vertex(c)).ToList();

            // Add all the commits
            graph.AddVertexRange(commitVertices);

            // Links all the commits
            foreach (var commitVertex in commitVertices.Where(c => c.Commit.ParentHashes != null))
            {
                foreach (var parentHash in commitVertex.Commit.ParentHashes)
                {
                    var parentVertex = commitVertices.SingleOrDefault(c => c.Commit.Hash == parentHash);
                    if (parentVertex != null) graph.AddEdge(new CommitEdge(commitVertex, parentVertex));
                }
            }

            // Add and link all the refs
            commitVertices
                .Where(c => c.Commit.Refs != null)
                .SelectMany(c => c.Commit.Refs.Select(r => new
                {
                    CommitVertex = c,
                    RefName = r
                }))
                .ToList()
                .ForEach(r =>
                {
                    var refVertex = new Vertex(new Reference {Name = r.RefName});
                    graph.AddVertex(refVertex);
                    graph.AddEdge(new CommitEdge(refVertex, r.CommitVertex));
                });

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
