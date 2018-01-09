using System;
using ApacheLogParser.Common.Services.Interfaces;

namespace ApacheLogParser.BL.Services
{
    /// <summary>
    /// Log to Console. Just example of using ILogger. Can be log4net etc.
    /// </summary>
    public class ConsoleLogger : ILogger
    {
        public void Error(string message)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(message);
        }

        public void Info(string message)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(message);
        }

        public void Debug(string message)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(message);
        }
    }
}
