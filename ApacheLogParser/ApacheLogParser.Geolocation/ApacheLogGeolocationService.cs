using ApacheLogParser.Common.Services.Interfaces;
using ApacheLogParser.DAL.Repositories;
using ApacheLogParser.Geolocation.Models;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Net;

namespace ApacheLogParser.Geolocation
{
    /// <summary>
    /// Geolocate apache logs by ip address.
    /// Separate service because of performance affection.
    /// It takes a lot of time if geolocate apache log during the data parsing, 
    /// so i desided to create a scheduled service which is gonna be ran each 1 hours (for example) and get all logs which haven't been geolocated and 
    /// via 3-d party api set country for them.
    /// </summary>
    public class ApacheLogGeolocationService
    {
        private readonly IApacheLogRepository _repository;
        private readonly ILogger _logger;

        public ApacheLogGeolocationService(IApacheLogRepository repository, ILogger logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public void Geolocate()
        {
            var logs = _repository.GetApacheLogsWitoutGeolocation().AsParallel();

            var geolocatedLogs = logs.Select(log =>
         {
             log.Geolocation = GetUserCountryByIp(log.Client);
             if (log.Geolocation != null)
                 _logger.Info($"Geolocation has been resolved for IP: IP: {log.Client} | Geolocation: {log.Geolocation}");
             return log;
         });


            try
            {
                _repository.UpdateGeolocationByClient(geolocatedLogs);
            }
            catch (Exception)
            {
                //log just for example. there can be a lot of useful parameters.
                _logger.Error("Error has occured while updating geolocation");
                throw;
            }
        }

        public static string GetUserCountryByIp(string ip)
        {
            var countryInfo = new CountryInfo();
            try
            {
                var info = new WebClient().DownloadString("http://ipinfo.io/" + ip);

                countryInfo = JsonConvert.DeserializeObject<CountryInfo>(info);

            }
            catch (Exception)
            {
                countryInfo.Country = null;
            }

            return countryInfo.Country;
        }
    }
}
