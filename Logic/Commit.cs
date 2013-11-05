namespace GitViz.Logic
{
    public class Commit
    {
        public string Hash { get; set; }
        public string[] ParentHashes { get; set; }
        public string[] Refs { get; set; }

        public override string ToString()
        {
            return Hash;
        }
    }
}
