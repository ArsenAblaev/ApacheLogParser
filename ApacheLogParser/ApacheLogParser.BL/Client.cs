using ApacheLogParser.BL.Parsers.Base;

namespace ApacheLogParser.BL
{
    public class Client
    {
        private readonly IParser _parser;

        public Client(IParser parser)
        {
            _parser = parser;
        }

        public void Parse()
        {
            _parser.Parse();
        }
    }
}
