using System;

namespace ApacheLogParser.Entities.Entities
{
    public class ApacheLog : IEntity
    {
        public int Id { get; set; }
        public DateTime RequestDate { get; set; }
        public string Client { get; set; }
        public string Route { get; set; }
        public string QueryParams { get; set; }
        public short StatusCode { get; set; }
        public int Size { get; set; }
        public string Geolocation { get; set; }
    }
}
