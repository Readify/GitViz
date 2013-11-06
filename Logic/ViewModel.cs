using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
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

                    RefreshGraph(logRetriever);

                    _watcher = new FileSystemWatcher(Path.Combine(_repositoryPath, @".git\refs"))
                    {
                        EnableRaisingEvents = true,
                        IncludeSubdirectories = true
                    };
                    var lag = new Timer(state => RefreshGraph(logRetriever), null, Timeout.Infinite, Timeout.Infinite);
                    FileSystemEventHandler onChanged = (sender, args) => lag.Change(TimeSpan.FromMilliseconds(100), Timeout.InfiniteTimeSpan);
                    _watcher.Changed += onChanged;
                    _watcher.Created += onChanged;
                    _watcher.Deleted += onChanged;
                }
                else
                {
                    _graph = new CommitGraph();
                    OnPropertyChanged("Graph");
                    if (_watcher != null) _watcher.Dispose();
                }
            }
        }

        void RefreshGraph(LogRetriever logRetriever)
        {
            var commits = logRetriever.GetRecentCommits();
            var activeRefName = logRetriever.GetActiveReferenceName();
            _graph = GenerateGraphFromCommits(commits, activeRefName);
            OnPropertyChanged("Graph");
        }

        CommitGraph GenerateGraphFromCommits(IEnumerable<Commit> commits, string activeRefName)
        {
            commits = commits.ToList();

            var graph = new CommitGraph();

            var commitVertices = commits.Select(c => new Vertex(c)).ToList();

            // Add all the vertices
            foreach (var commitVertex in commitVertices)
            {
                graph.AddVertex(commitVertex);

                if (commitVertex.Commit.Refs == null) continue;
                foreach (var refName in commitVertex.Commit.Refs)
                {
                    var refVertex = new Vertex(new Reference
                    {
                        Name = refName,
                        IsActive = refName == activeRefName
                    });
                    graph.AddVertex(refVertex);
                    graph.AddEdge(new CommitEdge(refVertex, commitVertex));
                }
            }

            // Add all the edges
            foreach (var commitVertex in commitVertices.Where(c => c.Commit.ParentHashes != null))
            {
                foreach (var parentHash in commitVertex.Commit.ParentHashes)
                {
                    var parentVertex = commitVertices.SingleOrDefault(c => c.Commit.Hash == parentHash);
                    if (parentVertex != null) graph.AddEdge(new CommitEdge(commitVertex, parentVertex));
                }
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
