using System.Collections.Generic;

namespace ApacheLogParser.BL.Services.Interfaces
{
    /// <summary>
    /// Provides wrapper over the static class file in order to unit testing.
    /// </summary>
    public interface IFile
    {
        IEnumerable<string> ReadLines(string filePath);
    }
}
