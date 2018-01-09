using System.Collections.Generic;
using System.IO;
using ApacheLogParser.BL.Services.Interfaces;

namespace ApacheLogParser.BL.Services
{
    /// <summary>
    /// Provides wrapper over the static class file in order to unit testing.
    /// </summary>
    public class FileWrapper : IFile
    {
        public IEnumerable<string> ReadLines(string filePath)
        {
            return File.ReadLines(filePath);
        }
    }
}
