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

            var commitVertices = commits.Select(c => new Vertex(c)).ToArray();

            graph.AddVertexRange(commitVertices);
            foreach (var commitVertex in commitVertices.Where(c => c.Commit.ParentHashes != null))
            {
                foreach (var parentHash in commitVertex.Commit.ParentHashes)
                {
                    var parentVertex = commitVertices.SingleOrDefault(c => c.Commit.Hash == parentHash);
                    if (parentVertex != null) graph.AddEdge(new CommitEdge(commitVertex, parentVertex));
                }
            }

            foreach (var commitVertex in commitVertices.Where(c => c.Commit.Refs != null))
                foreach (var refName in commitVertex.Commit.Refs)
                {
                    var refVertex = new Vertex(new Reference { Name = refName });
                    graph.AddVertex(refVertex);
                    graph.AddEdge(new CommitEdge(refVertex, commitVertex));
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
