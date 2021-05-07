using System.Text.Json.Serialization;

namespace safuCHARTS.Models
{
    public class LogEventDecodedParam
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("type")]
        public string Type { get; set; }

        [JsonPropertyName("indexed")]
        public bool Indexed { get; set; }

        [JsonPropertyName("decoded")]
        public bool Decoded { get; set; }

        [JsonPropertyName("value")]
        public string Value { get; set; }
    }
}