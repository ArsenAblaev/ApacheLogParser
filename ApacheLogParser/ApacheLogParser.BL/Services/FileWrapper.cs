using System.Collections.Generic;
using System.IO;
using ApacheLogParser.BL.Services.Interfaces;

namespace ApacheLogParser.BL.Services
{
    public class FileWrapper : IFile
    {
        public IEnumerable<string> ReadLines(string filePath)
        {
            return File.ReadLines(filePath);
        }
    }
}
