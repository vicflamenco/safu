using System.Text.Json.Serialization;

namespace safuCHARTS.Models
{
    public class Bar // time, open, high, low, close, volume }
    {
        [JsonPropertyName("time")]
        public long Time { get; set; }

        [JsonPropertyName("open")]
        public double Open { get; set; }

        [JsonPropertyName("high")]
        public double High { get; set; }

        [JsonPropertyName("low")]
        public double Low { get; set; }

        [JsonPropertyName("close")]
        public double Close { get; set; }

        [JsonPropertyName("volume")]
        public double Volume { get; set; }
    }
}