using Newtonsoft.Json;

namespace MuseoLibrary.ApplicationDomain.Models
{
    public class Stamp
    {
        [JsonProperty("id")]
        public string id { get; set; } = string.Empty;

        [JsonProperty("name")]
        public string Name { get; set; } = string.Empty;

        [JsonProperty("location")]
        public Location Location { get; set; } = new();

        [JsonProperty("description")]
        public string Description { get; set; } = string.Empty;

        [JsonProperty("userImages")]
        public List<string> UserImages { get; set; } = new();
    }
}
