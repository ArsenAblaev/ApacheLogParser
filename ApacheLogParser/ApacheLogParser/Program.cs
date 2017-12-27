﻿using ApacheLogParser.BL;
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
            var repository = new ApacheLogRepository();
            ILogger logger = new ConsoleLogger();
            IFile file = new FileWrapper();
            IParser parser = new ApacheParser
                (@"F:\Projects\ApacheLogParser\Files\access_log_Jul95",
                repository,
                logger,
                file);
            var client = new Client(parser);
            client.Parse();
        }
    }
}
