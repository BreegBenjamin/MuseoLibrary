using Newtonsoft.Json;

namespace MuseoLibrary.UploadImagesService.Model
{
    public class ImageMessage
    {
        [JsonProperty("userId")]
        public string UserId { get; set; } = Guid.NewGuid().ToString();

        [JsonProperty("tripId")]
        public string TripId { get; set; } = Guid.NewGuid().ToString();

        [JsonProperty("fileName")]
        public string FileName { get; set; } = string.Empty;

        [JsonProperty("imageUrl")]
        public string ImageUrl { get; set; } = string.Empty;

    } 
}