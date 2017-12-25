using System.Collections.Generic;

namespace ApacheLogParser.BL.Services.Interfaces
{
    public interface IFile
    {
        IEnumerable<string> ReadLines(string filePath);
    }
}
