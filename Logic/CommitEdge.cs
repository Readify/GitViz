using QuickGraph;

namespace GitViz.Logic
{
    public class CommitEdge : Edge<Vertex>
    {
        public CommitEdge(Vertex source, Vertex target)
            : base(source, target)
        {
        }
    }
}
