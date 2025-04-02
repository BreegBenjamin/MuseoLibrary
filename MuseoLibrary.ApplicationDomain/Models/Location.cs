using Newtonsoft.Json;

namespace MuseoLibrary.ApplicationDomain.Models
{
    public class Location
    {
        [JsonProperty("lat")]
        public double Lat { get; set; }

        [JsonProperty("lon")]
        public double Lon { get; set; }
    }
}
