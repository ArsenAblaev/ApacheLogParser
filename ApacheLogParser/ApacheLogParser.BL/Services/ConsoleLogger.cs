using System;
using ApacheLogParser.BL.Services.Interfaces;

namespace ApacheLogParser.BL.Services
{
    public class ConsoleLogger : ILogger
    {
        public void Error(string message)
        {
            Console.WriteLine(message);
        }

        public void Info(string message)
        {
            Console.WriteLine(message);
        }

        public void Debug(string message)
        {
            Console.WriteLine(message);
        }
    }
}
