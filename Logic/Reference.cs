namespace GitViz.Logic
{
    public class Reference
    {
        public static string HEAD = "HEAD";

        public string Name { get; set; }
        public bool IsActive { get; set; }
        public bool IsHead { get { return Name == HEAD; } }
        public bool IsTag { get { return Name.StartsWith("tag:"); } }
        public bool IsRemote { get { return Name.Contains("/");} }
    }
}
