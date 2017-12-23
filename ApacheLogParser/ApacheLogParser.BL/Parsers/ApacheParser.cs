using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ApacheLogParser.BL.Parsers.Base;
using ApacheLogParser.Entities.Entities;

namespace ApacheLogParser.BL.Parsers
{
    public class ApacheParser : IParser
    {
        private readonly object _lock = new object();

        public void Parse()
        {
            var lines = File.ReadLines(@"E:\Plarium\apache-samples\access_log\access_log_Jul95").Take(100000).AsParallel();

            var logEntryPattern = "^([^\\>]+) (\\S+) (\\S+) \\[([\\w:/]+\\s[+\\-]\\d{4})\\] \"(.+?)\" (\\d{3}) (\\d+|-)";


            var regEx = new Regex(logEntryPattern);

            //   (?:[A - Za - z0 - 9][A - Za - z0 - 9\-]{ 0,61}[A-Za-z0-9]|[A-Za-z0-9])

            //Console.WriteLine("IP Address: " + regexMatch.Groups[1].Value);
            //Console.WriteLine("Date&Time: " + regexMatch.Groups[4].Value);
            //Console.WriteLine("Request: " + regexMatch.Groups[5].Value);
            //Console.WriteLine("Response: " + regexMatch.Groups[6].Value);
            //Console.WriteLine("Bytes Sent: " + regexMatch.Groups[7].Value);

            var stopWatch = new Stopwatch();
            stopWatch.Start();


            var result = lines.Where(x => regEx.IsMatch(x)).Select(x =>
             {

                 var regexMatch = regEx.Match(x);
                 var log = new ApacheLog
                 {
                     Ip = regexMatch.Groups[1].Value,
                     //RequestDate = DateTime.Parse(regexMatch.Groups[4].Value),
                     Route = regexMatch.Groups[5].Value,
                     StatusCode = (HttpStatusCode)int.Parse(regexMatch.Groups[6].Value),
                     Size = int.TryParse(regexMatch.Groups[7].Value, out int size) ? size : default(int)
                 };
                 return log;

             }).ToList();


            stopWatch.Stop();
            Console.WriteLine();

            Console.WriteLine($"Count: {result.Count} | Time: {stopWatch.Elapsed}");
        }
    }
}
