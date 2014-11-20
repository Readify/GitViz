namespace GitViz.Logic
{
    public class Reference
    {
        public static string HEAD = "HEAD";

        public string Name { get; set; }
        public bool IsActive { get; set; }
        public bool IsHead { get { return Name == HEAD; } }
    }
}
