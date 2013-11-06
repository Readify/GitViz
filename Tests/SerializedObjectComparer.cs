using System.Collections;

namespace GitViz.Tests
{
    public class SerializedObjectComparer : IComparer
    {
        public int Compare(object x, object y)
        {
            return string.CompareOrdinal(x.ToJson(), y.ToJson());
        }
    }
}
