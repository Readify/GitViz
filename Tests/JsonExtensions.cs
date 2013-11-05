using System.IO;
using Newtonsoft.Json;

namespace GitViz.Tests
{
    static class JsonExtensions
    {
        internal static string ToJson(this object value)
        {
            using (var stringWriter = new StringWriter())
            {
                var jsonSerializer = new JsonSerializer { NullValueHandling = NullValueHandling.Ignore };
                jsonSerializer.Serialize(stringWriter, value);
                return stringWriter.ToString().Replace("\"", "");
            }
        }
    }
}
