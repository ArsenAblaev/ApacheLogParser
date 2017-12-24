using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using ApacheLogParser.BL.Parsers.Base;
using ApacheLogParser.DAL.Repositories;
using ApacheLogParser.DAL.Repositories.Base;
using ApacheLogParser.Entities.Entities;

namespace ApacheLogParser.BL.Parsers
{
    public class ApacheParser : IParser
    {
        private readonly string _filePath;
        private readonly IApacheLogRepository _repository;

        public ApacheParser(string filePath, IApacheLogRepository repository)
        {
            _filePath = filePath;
            _repository = repository;
        }
      
        public  void Parse()
        {
            var lines = File.ReadLines(_filePath)//.Take(100000)
                .AsParallel();


            // var count = lines.Count();
            var logEntryPattern = "^([^\\>]+) (\\S+) (\\S+) \\[([\\w:/]+\\s[+\\-]\\d{4})\\] \"(((?!gif|GIF|jpg|JPG).)+?)\" (\\d{3}) (\\d+|-)";


            //Example todo
            //crystal.ipac.caltech.edu - - [17/Jul/1995:20:00:23 -0400] "GET /facts/faq04.html HTTP/1.0" 200 27063


            var regEx = new Regex(logEntryPattern);


           var result = lines.Where(x => regEx.IsMatch(x)).Select(x =>
             {

                 var regexMatch = regEx.Match(x);
                 var log = new ApacheLog
                 {
                     Ip = regexMatch.Groups[1].Value,
                     RequestDate = DateTime.Now,// DateTime.Parse(regexMatch.Groups[4].Value),
                     Route = regexMatch.Groups[5].Value,
                     StatusCode = short.Parse(regexMatch.Groups[7].Value),
                     Size = int.TryParse(regexMatch.Groups[8].Value, out int size) ? size : default(int)
                 };

                 return log;

             }).ToList();

            var stopWatch = new Stopwatch();
            stopWatch.Start();
            
            _repository.BulkInsert(result);

            stopWatch.Stop();
            Console.WriteLine();


            // Time: 11
            Console.WriteLine($"Count: {result.Count} | Time: {stopWatch.Elapsed}");
        }
    }
}
