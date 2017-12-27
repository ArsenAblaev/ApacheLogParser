using System;
using System.Collections.Concurrent;
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
using ApacheLogParser.Common.Services.Interfaces;
using ApacheLogParser.DAL.Repositories;
using ApacheLogParser.DAL.Repositories.Base;
using ApacheLogParser.Entities.Entities;
using MaxMind.GeoIP2;
using Newtonsoft.Json;

namespace ApacheLogParser.BL.Parsers
{
    public class ApacheParser : IParser
    {
        private readonly string _filePath;
        private readonly IApacheLogRepository _repository;
        private readonly ILogger _logger;
        private readonly IFile _file;


        private static object _lock = new object();

        private const string LogEntryPattern = "^([^\\>]+) (\\S+) (\\S+) \\[([\\w:/]+\\s[+\\-][0-9][0-9][0-9][0-9])\\] \"(((?!.gif|.GIF|.jpg|.JPG|.xbm).)+?)\" ([0-9][0-9][0-9]) (\\d+|-)";

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


            var regEx = new Regex(LogEntryPattern);
            // var invalid = lines.Where(x => regEx.Match(x).Groups[5].Value.Split(' ').Length < 2).ToList();

            const string datePattern = "dd/MMM/yyyy:HH:mm:ss zzz";

            var result2 = lines.Select(x =>
               {

                   var match = regEx.Match(x);

                   if (string.IsNullOrEmpty(match.Value)) return null;

                   try
                   {
                       var log = new ApacheLog
                       {
                           Client = match.Groups[1].Value,
                           RequestDate = DateTime.ParseExact(match.Groups[4].Value, datePattern, CultureInfo.InvariantCulture),
                           Route = match.Groups[5].Value,
                           QueryParams = match.Groups[5].Value.Split(' ').Length > 1 ? string.Join(", ", match.Groups[5].Value.Split(' ')[1].Split('?').Skip(1)) : null,
                           StatusCode = short.Parse(match.Groups[7].Value),
                           Size = int.TryParse(match.Groups[8].Value, out int size) ? size : default(int)
                       };

                       return log;
                   }
                   catch (Exception e)
                   {
                       Console.WriteLine($"Route:  {match.Groups[5].Value}");
                       throw;
                   }
                   



              }).Where(x => x != null).ToList();







            stopWatch.Stop();

            Console.WriteLine("Data was mapped");
            Console.WriteLine($"Time: {stopWatch.Elapsed}");
            stopWatch.Restart();

            //   return;

            Console.WriteLine("Start inserting...");
            _repository.BulkInsert(result2);

            stopWatch.Stop();

            Console.WriteLine("Data was sucessfully inserted");


            // Time: 11
            Console.WriteLine($"Count: {result2.Count} | Time: {stopWatch.Elapsed}");
        }
    }


}
