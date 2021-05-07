using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace safuCHARTS.Models
{
    public class PricingDataContentItem
    {
        [JsonPropertyName("quote_rate")]
        public double? QuoteRate { get; set; }
    }
}