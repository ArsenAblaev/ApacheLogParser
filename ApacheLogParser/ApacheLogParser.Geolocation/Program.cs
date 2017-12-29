using System;
using ApacheLogParser.BL.Services;
using ApacheLogParser.Common.Services.Interfaces;
using ApacheLogParser.DAL.Repositories;
using Timer = System.Threading.Timer;

namespace ApacheLogParser.Geolocation
{
    class Program
    {
        static void Main()
        {
            Console.WriteLine("Start geolocation sevice");

            IApacheLogRepository repository = new ApacheLogRepository();
            ILogger logger = new ConsoleLogger();
            var geolocationService = new ApacheLogGeolocationService(repository, logger);

            new Timer(state => geolocationService.Geolocate(), "Geolocate", 0, 60 * 60 * 1000);
            
            Console.ReadLine();
        }
    }
}
