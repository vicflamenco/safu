using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace safuCHARTS.Models
{
    public class TransactionDataContent
    {
        [JsonPropertyName("address")]
        public string Address { get; set; }

        [JsonPropertyName("updated_at")]
        public string UpdatedAt { get; set; }

        [JsonPropertyName("next_update_at")]
        public string NextUpdateAt { get; set; }

        [JsonPropertyName("quote_currency")]
        public string QuoteCurrency { get; set; }

        [JsonPropertyName("chain_id")]
        public int? ChainID { get; set; }

        [JsonPropertyName("items")]
        public List<TransactionDataContentItem> Items { get; set; }

        [JsonPropertyName("pagination")]
        public Pagination Pagination { get; set; }
    }
}
