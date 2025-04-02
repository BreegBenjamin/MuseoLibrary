using Newtonsoft.Json;

namespace MuseoLibrary.ApplicationDomain.DTOs
{
    public class UserDto
    {
        [JsonProperty("id")]
        public string id { get; set; } = Guid.NewGuid().ToString();

        [JsonProperty("name")]
        public string Name { get; set; } = string.Empty;

        [JsonProperty("userName")]
        public string UserName { get; set; } = string.Empty;

        [JsonProperty("email")]
        public string Email { get; set; } = string.Empty;

        [JsonProperty("password")]
        public string Password { get; set; } = string.Empty;

        [JsonProperty("userDescription")]
        public string UserDescription { get; set; } = string.Empty;

        [JsonProperty("userImageProfile")]
        public string UserImageProfile { get; set; } = string.Empty;
    }
}
