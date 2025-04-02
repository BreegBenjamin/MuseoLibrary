using Newtonsoft.Json;

namespace MuseoLibrary.ApplicationDomain.DTOs
{
    public class TripDto
    {
        [JsonProperty("userId")]
        public string UserId { get; set; } = string.Empty;

        [JsonProperty("place")]
        public string PlaceDescription { get; set; } = string.Empty;

        [JsonProperty("imageUrl")]
        public string ImageURL { get; set; } = string.Empty;

    }
}
