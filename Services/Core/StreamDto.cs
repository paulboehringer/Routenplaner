using System.IO;

namespace Services.Core
{
    /// <summary>
    /// Data transfer object to transfer gif from google api
    /// </summary>
    public class StreamDto
    {
        public Stream Stream { get; set; }
        public string Error { get; set; }
    }
}
