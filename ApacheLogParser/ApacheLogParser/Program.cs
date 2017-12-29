using System;
using System.IO;
using ApacheLogParser.BL;
using ApacheLogParser.BL.Parsers;
using ApacheLogParser.BL.Parsers.Base;
using ApacheLogParser.BL.Services;
using ApacheLogParser.BL.Services.Interfaces;
using ApacheLogParser.Common.Services.Interfaces;
using ApacheLogParser.DAL.Repositories;

namespace ApacheLogParser
{
    class Program
    {
        static void Main()
        {
            IApacheLogRepository repository = new ApacheLogRepository();
            ILogger logger = new ConsoleLogger();
            IFile file = new FileWrapper();
            IParser parser = new ApacheParser
            (TryToGetFilePath(),
            repository,
            logger,
            file);
            var client = new Client(parser);
            client.Parse();
        }

        static string TryToGetFilePath()
        {
            Console.WriteLine("Please choose a file...");
            var filePath = Console.ReadLine();
            while (!File.Exists(filePath))
            {
                Console.WriteLine("File does not exist. Please choose another path");
                filePath = Console.ReadLine();
            }
            return filePath;
        }
    }
}
