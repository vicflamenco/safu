using System.Text.Json.Serialization;

namespace safuCHARTS.Models
{
    public class HistoricDataItem
    {
        [JsonPropertyName("time")]
        public long Time { get; set; }

        [JsonPropertyName("value")]
        public double Value { get; set; }
    }
}