using System;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using ApacheLogParser.BL.Parsers.Base;
using ApacheLogParser.BL.Services.Interfaces;
using ApacheLogParser.Common.Services.Interfaces;
using ApacheLogParser.DAL.Repositories;
using ApacheLogParser.Entities.Entities;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace ApacheLogParser.BL.Parsers
{
    /// <summary>
    /// Parse apache logs from source file and insert them to database.
    /// </summary>
    public class ApacheParser : IParser
    {
        private readonly string _filePath;
        private readonly IApacheLogRepository _repository;
        private readonly ILogger _logger;
        private readonly IFile _file;

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
            var stopWatch = new Stopwatch();
            stopWatch.Start();

            ParallelQuery<string> lines;
            try
            {
                _logger.Info($"Trying to read souce log file. Path={_filePath}");
                lines = _file.ReadLines(_filePath).AsParallel();
            }
            catch (Exception)
            {
                _logger.Error($"Error has occured while reading file. Path={_filePath}");
                throw;
            }
           
            var regEx = new Regex(LogEntryPattern);

            const string datePattern = "dd/MMM/yyyy:HH:mm:ss zzz";

            List<ApacheLog> apacheLogs;
            try
            {
                _logger.Info("Trying to parse apache logs...");

                apacheLogs = lines.Select(x =>
               {
                   var match = regEx.Match(x);

                   if (string.IsNullOrEmpty(match.Value)) return null;

                   var log = new ApacheLog
                   {
                       Client = match.Groups[1].Value,
                       RequestDate = DateTime.ParseExact(match.Groups[4].Value, datePattern, CultureInfo.InvariantCulture),
                       Route = match.Groups[5].Value,
                       //get QueryParams from route by splitting by '?' and join to one string example result is  Route = /route/ex?251?231 QueryParam = 251, 231
                       QueryParams = match.Groups[5].Value.Split(' ').Length > 1 ? string.Join(", ", match.Groups[5].Value.Split(' ')[1].Split('?').Skip(1)) : null,
                       StatusCode = short.Parse(match.Groups[7].Value),
                       Size = int.TryParse(match.Groups[8].Value, out int size) ? size : default(int)
                   };

                   return log;

               }).Where(x => x != null).ToList();

                _logger.Info("Apache logs have been parsed successfully.");
            }
            catch (Exception)
            {
                _logger.Error("Error has occured while parsing apache logs");
                throw;
            }

            stopWatch.Stop();
            Console.WriteLine($"Time: {stopWatch.Elapsed}");
            stopWatch.Restart();
            
            try
            {
                _logger.Info("Start inserting logs...");
                _repository.BulkInsert(apacheLogs);
                _logger.Info("Logs have been inserted successfully");
            }
            catch (SqlException exception)
            {
                _logger.Error($"DataBase error: Error has occured while inserting logs to database. Message={exception.Message}");
                throw;
            }
            catch (Exception exception)
            {
                _logger.Error($"Internal Server Error: Error has occured while inserting logs to database. Message={exception}");
                throw;
            }

            stopWatch.Stop();

            Console.WriteLine("Data was sucessfully inserted");

            Console.WriteLine($"Count: {apacheLogs.Count} | Time: {stopWatch.Elapsed}");
        }
    }
}
