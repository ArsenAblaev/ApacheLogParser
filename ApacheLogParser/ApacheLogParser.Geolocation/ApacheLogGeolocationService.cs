using ApacheLogParser.Common.Services.Interfaces;
using ApacheLogParser.DAL.Repositories;
using ApacheLogParser.Geolocation.Models;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Net;

namespace ApacheLogParser.Geolocation
{
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
                 _logger.Info($"Geolocation has been resolved : {log.Geolocation}");
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
            var ipInfo = new IpInfo();
            try
            {
                var info = new WebClient().DownloadString("http://ipinfo.io/" + ip);

                ipInfo = JsonConvert.DeserializeObject<IpInfo>(info);

            }
            catch (Exception)
            {
                ipInfo.Country = null;
            }

            return ipInfo.Country;
        }
    }
}
