using Newtonsoft.Json;
using System.IO;

namespace MuseoLibrary.ApplicationDomain.Models
{
    public class Trip
    {
        [JsonProperty("id")]
        public string id { get; set; } = Guid.NewGuid().ToString();

        [JsonProperty("userId")]
        public string UserId { get; set; } = string.Empty;

        [JsonProperty("location")]
        public Location Location { get; set; } = new();

        [JsonProperty("images")]
        public List<string> Images { get; set; } = new();

        [JsonProperty("validatedByAI")]
        public bool ValidatedByAI { get; set; } = false;
        public float Confidence { get; set; }
        public DateTime Date { get; set; } = DateTime.UtcNow;
        public string Description { get; set; } = string.Empty;
        public List<string>? Tags { get; set; }
        public List<string>? Categories { get; set; }
        public bool IsAdultContent { get; set; }
        public bool IsRacyContent { get; set; }
        public bool IsViolentContent { get; set; }
        public List<DetectedObjectInfo>? DetectedObjects { get; set; } = new();
        public List<FaceInfo>? Faces { get; set; }
    }

    public class DetectedObjectInfo
    {
        public string? ObjectText { get; set; }
        public double Confidence { get; set; }
    }

    public class FaceInfo
    {
        public int Age { get; set; }
        public string? Gender { get; set; }
    }
}
