using QuickGraph;

namespace GitViz.Logic
{
    public class CommitEdge : Edge<Commit>
    {
        public CommitEdge(Commit source, Commit target)
            : base(source, target)
        {
        }
    }
}
