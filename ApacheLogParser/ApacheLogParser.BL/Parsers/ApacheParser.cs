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

            int count = 0;
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
                     
                          count++;

                      //todo Add service to Geolocate
                     
                      //var geolocation = GetUserCountryByIp(log.Client);

                      //if (geolocation != null)
                      //{
                      //    Console.WriteLine($"Country: {geolocation}, Client: {log.Client}, Count: {count}");
                      //}

                      return log;
                  }
                  catch (Exception e)
                  {
                      Console.WriteLine($"Route:  {match.Groups[5].Value}");
                      throw;
                  }




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

        public static string GetUserCountryByIp(string ip)
        {


            IpInfo ipInfo = new IpInfo();
            try
            {
                var info = new WebClient().DownloadString("http://ipinfo.io/" + ip);

                ipInfo = JsonConvert.DeserializeObject<IpInfo>(info);
                var myRI1 = new RegionInfo(ipInfo.Country);
                ipInfo.Country = myRI1.EnglishName;
            }
            catch (Exception e)
            {
                ipInfo.Country = null;
            }

            return ipInfo.Country;
        }


    }

    public class IpInfo
    {

        //[JsonProperty("ip")]
        //public string Ip { get; set; }

        //[JsonProperty("hostname")]
        //public string Hostname { get; set; }

        //[JsonProperty("city")]
        //public string City { get; set; }

        //[JsonProperty("region")]
        //public string Region { get; set; }

        [JsonProperty("country")]
        public string Country { get; set; }

        //[JsonProperty("loc")]
        //public string Loc { get; set; }

        //[JsonProperty("org")]
        //public string Org { get; set; }

        //[JsonProperty("postal")]
        //public string Postal { get; set; }
    }

}
