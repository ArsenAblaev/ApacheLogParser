using ApacheLogParser.Common.Services.Interfaces;
using log4net;
using log4net.Config;

namespace ApacheLogParser.API.Infrastructure.Services
{
    public class Logger : ILogger
    {
        private static readonly ILog Log = LogManager.GetLogger("LOGGER");
        
        public static void InitLogger()
        {
            XmlConfigurator.Configure();
        }

        public void Error(string message)
        {
            Log.Error(message);
        }

        public void Info(string message)
        {
            Log.Info(message);
        }

        public void Debug(string messaga)
        {
            Log.Debug(messaga);
        }
    }
}