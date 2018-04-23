using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using GitViz.Logic.Annotations;
using System;
using System.Text.RegularExpressions;

namespace GitViz.Logic
{
    public class ViewModel : INotifyPropertyChanged
    {
        string _repositoryPath = "";
        CommitGraph _graph = new CommitGraph();
        RepositoryWatcher _watcher;

        readonly LogParser _parser = new LogParser();

        public string WindowTitle
        {
            get
            {
                return "Readify GitViz (Alpha)"
                    + (string.IsNullOrWhiteSpace(_repositoryPath)
                        ? string.Empty
                        : " - " + Path.GetFileName(_repositoryPath));
            }
        }

        public bool IsNewRepository { get; set; }
        public string RepositoryPath
        {
            get { return _repositoryPath; }
            set
            {
                _repositoryPath = value.TrimEnd();
                if (IsValidGitRepository(_repositoryPath))
                {
                    OnPropertyChanged("WindowTitle");
                    var commandExecutor = new GitCommandExecutor(_repositoryPath);
                    var logRetriever = new LogRetriever(commandExecutor, _parser);

                    RefreshGraph(logRetriever);

                    IsNewRepository = true;

                    _watcher = new RepositoryWatcher(_repositoryPath, IsBareGitRepository(_repositoryPath));
                    _watcher.ChangeDetected += (sender, args) => RefreshGraph(logRetriever);
                }
                else
                {
                    _graph = new CommitGraph();
                    OnPropertyChanged("Graph");
                    if (_watcher != null)
                    {
                        _watcher.Dispose();
                        _watcher = null;
                    }
                }
            }
        }

        void RefreshGraph(LogRetriever logRetriever)
        {
            var commits = logRetriever.GetRecentCommits().ToArray();
            var activeRefName = logRetriever.GetActiveReferenceName();

            var reachableCommitHashes = commits.Select(c => c.Hash).ToArray();
            var unreachableHashes = logRetriever.GetRecentUnreachableCommitHashes();
            var unreachableCommits = logRetriever
                .GetSpecificCommits(unreachableHashes)
                .Where(c => !reachableCommitHashes.Contains(c.Hash))
                .ToArray();

            _graph = GenerateGraphFromCommits(commits, activeRefName, unreachableCommits);
            OnPropertyChanged("Graph");
        }

        CommitGraph GenerateGraphFromCommits(IEnumerable<Commit> commits, string activeRefName, IEnumerable<Commit> unreachableCommits)
        {
            commits = commits.ToList();

            var graph = new CommitGraph();

            var commitVertices = commits.Select(c => new Vertex(c))
                .Union(unreachableCommits.Select(c => new Vertex(c) { Orphan = true }))
                .ToList();

            // Add all the vertices
            var headVertex = new Vertex(new Reference
            {
               Name = Reference.HEAD,
            });

            foreach (var commitVertex in commitVertices)
            {
                graph.AddVertex(commitVertex);

                if (commitVertex.Commit.Refs == null) continue;
                var isHeadHere = false;
                var isHeadSet = false;
                foreach (var refName in commitVertex.Commit.Refs)
                {
                    if (refName == Reference.HEAD)
                    {
                        isHeadHere = true;
                        graph.AddVertex(headVertex);
                        continue;
                    }
                    var refVertex = new Vertex(new Reference
                    {
                        Name = refName,
                        IsActive = refName == activeRefName
                    });
                    graph.AddVertex(refVertex);
                    graph.AddEdge(new CommitEdge(refVertex, commitVertex));
                    if (!refVertex.Reference.IsActive) continue;
                    isHeadSet = true;
                    graph.AddEdge(new CommitEdge(headVertex, refVertex));
                }
                if (isHeadHere && !isHeadSet)
                    graph.AddEdge(new CommitEdge(headVertex, commitVertex));
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
            return !string.IsNullOrEmpty(path)
                && Directory.Exists(path)
                && (Directory.Exists(Path.Combine(path, ".git")) ||
                 IsBareGitRepository(path));
        }

        static Boolean IsBareGitRepository(String path) 
        {
            String configFileForBareRepository = Path.Combine(path, "config"); 
            return File.Exists(configFileForBareRepository) &&
                  Regex.IsMatch(File.ReadAllText(configFileForBareRepository), @"bare\s*=\s*true", RegexOptions.IgnoreCase);
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
