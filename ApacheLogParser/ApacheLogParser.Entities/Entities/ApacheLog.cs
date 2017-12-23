using System;
using System.Net;

namespace ApacheLogParser.Entities.Entities
{
    public class ApacheLog : IEntity
    {
        public int Id { get; set; }
        public DateTime RequestDate { get; set; }
        public string Ip { get; set; }
        public string Route { get; set; }
        public string QueryParams { get; set; }
        public HttpStatusCode StatusCode { get; set; }
        public int Size { get; set; }
    }
}
