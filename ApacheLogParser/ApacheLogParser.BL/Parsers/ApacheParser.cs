using System;
using System.Collections.Generic;
using System.Data;
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
using ApacheLogParser.BL.Services.Interfaces;
using ApacheLogParser.DAL.Repositories;
using ApacheLogParser.DAL.Repositories.Base;
using ApacheLogParser.Entities.Entities;

namespace ApacheLogParser.BL.Parsers
{
    public class ApacheParser : IParser
    {
        private readonly string _filePath;
        private readonly IApacheLogRepository _repository;
        private readonly ILogger _logger;
        private readonly IFile _file;

        private const string LogEntryPattern = "^([^\\>]+) (\\S+) (\\S+) \\[([\\w:/]+\\s[+\\-]\\d{4})\\] \"(((?!gif|GIF|jpg|JPG).)+?)\" (\\d{3}) (\\d+|-)";
        //private const string LogEntryPattern = "^([^\\>]+) (\\S+) (\\S+) \\[([\\w:/]+\\s[+\\-]\\d{4})\\] \"(((?!gif|GIF|jpg|JPG).)+?)|\\?(.*)\\s(.*)\" (\\d{3}) (\\d+|-)";

        public ApacheParser(string filePath, IApacheLogRepository repository, ILogger logger, IFile file)
        {
            _filePath = filePath;
            _repository = repository;
            _logger = logger;
            _file = file;
        }

        public void Parse()
        {
            Console.WriteLine("Start...");


            var stopWatch = new Stopwatch();
            stopWatch.Start();

            var lines = _file.ReadLines(_filePath)//.Take(100000)
                .AsParallel();


            //Example todo
            //crystal.ipac.caltech.edu - - [17/Jul/1995:20:00:23 -0400] "GET /facts/faq04.html HTTP/1.0" 200 27063


            var regEx = new Regex(LogEntryPattern);

            const string datePattern = "dd/MMM/yyyy:HH:mm:ss zzz";


            var result2 = lines.Select(x =>
            {

                // var splited = x.Split(' ');
                var regexMatch = regEx.Match(x);
                if (string.IsNullOrEmpty(regexMatch.Value)) return null;
                var log = new ApacheLog
                {
                    Client = regexMatch.Groups[1].Value,
                    RequestDate = DateTime.ParseExact(regexMatch.Groups[4].Value, datePattern, CultureInfo.InvariantCulture),
                    Route = regexMatch.Groups[5].Value,
                    //QueryParams = regexMatch.Groups[7].Value,
                    StatusCode = short.Parse(regexMatch.Groups[7].Value),
                    Size = int.TryParse(regexMatch.Groups[8].Value, out int size) ? size : default(int)
                };

                return log;

                // var queryParams = regexMatch.Groups[5].Value.Split('?');


                //var log = new ApacheLog
                //{
                //    Client = splited[0],
                //    RequestDate = DateTime.ParseExact($"{splited[3].Replace("[","")} {splited[4].Replace("]","")}",datePattern,CultureInfo.InvariantCulture),
                //    Route = splited[6],
                //    StatusCode = short.Parse(splited[8]),
                //    Size = int.TryParse(splited[9], out int size) ? size:default(int)
                //};



            }).Where(x => x != null).ToList();



            Console.WriteLine("Data was mapped");
            Console.WriteLine($"Time: {stopWatch.Elapsed}");
            stopWatch.Restart();


            Console.WriteLine("Start inserting...");
            _repository.BulkInsert(result2);

            stopWatch.Stop();

            Console.WriteLine("Data was sucessfully inserted");


            // Time: 11
            Console.WriteLine($"Count: {result2.Count} | Time: {stopWatch.Elapsed}");
        }


    }
}
