namespace GitViz.Logic
{
    public class Commit
    {
        public string Hash { get; set; }
        public string[] ParentHashes { get; set; }
    }
}
