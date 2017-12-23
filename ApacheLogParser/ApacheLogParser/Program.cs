using ApacheLogParser.BL;
using ApacheLogParser.BL.Parsers;
using ApacheLogParser.BL.Parsers.Base;

namespace ApacheLogParser
{
    class Program
    {
        static void Main(string[] args)
        {
            IParser parser = new ApacheParser();
            var client = new Client(parser);
            client.Parse();
        }
    }
}
