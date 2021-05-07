using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace safuCHARTS.Models
{
    public class PricingDataContent
    {
        [JsonPropertyName("updated_at")]
        public string UpdatedAt { get; set; }

        [JsonPropertyName("items")]
        public List<PricingDataContentItem> Items { get; set; }

        [JsonPropertyName("pagination")]
        public Pagination Pagination { get; set; }
    }
}
