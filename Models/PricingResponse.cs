using System.Text.Json.Serialization;

namespace safuCHARTS.Models
{
    public class PricingResponse
    {
        [JsonPropertyName("quote_rate")]
        public double? QuoteRate { get; set; }
    }
}