using Newtonsoft.Json;

namespace ApacheLogParser.Geolocation.Models
{
    /// <summary>
    /// Needed to get Country by ip address.
    /// </summary>
    public class CountryInfo
    {
        [JsonProperty("country")]
        public string Country { get; set; }
    }
}
