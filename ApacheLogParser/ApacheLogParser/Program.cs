using ApacheLogParser.BL;
using ApacheLogParser.BL.Parsers;
using ApacheLogParser.BL.Parsers.Base;
using ApacheLogParser.DAL.Repositories;

namespace ApacheLogParser
{
    class Program
    {
        static void Main(string[] args)
        {
            var repository = new ApacheLogRepository();
            IParser parser = new ApacheParser(@"E:\Plarium\apache-samples\access_log\access_log_Jul95", repository);
            var client = new Client(parser);
            client.Parse();
        }
    }
}
