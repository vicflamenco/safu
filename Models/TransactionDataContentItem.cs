using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace safuCHARTS.Models
{
    public class TransactionDataContentItem
    {
        [JsonPropertyName("block_signed_at")]
        public string BlockSignedAt { get; set; }

        [JsonPropertyName("tx_hash")]
        public string TxHash { get; set; }

        [JsonPropertyName("tx_offset")]
        public int TxOffset { get; set; }

        [JsonPropertyName("successful")]
        public bool Successful { get; set; }

        [JsonPropertyName("from_address")]
        public string FromAddress { get; set; }

        [JsonPropertyName("from_address_label")]
        public string FromAddressLabel { get; set; }

        [JsonPropertyName("to_address")]
        public string ToAddress { get; set; }

        [JsonPropertyName("to_address_label")]
        public string ToAddressLabel { get; set; }

        [JsonPropertyName("value")]
        public string Value { get; set; }

        [JsonPropertyName("value_quote")]
        public double ValueQuote { get; set; }

        [JsonPropertyName("gas_offered")]
        public double GasOffered { get; set; }

        [JsonPropertyName("gas_spent")]
        public double GasSpent { get; set; }

        [JsonPropertyName("gas_price")]
        public double GasPrice { get; set; }

        [JsonPropertyName("gas_quote")]
        public double GasQuote { get; set; }

        [JsonPropertyName("gas_quote_rate")]
        public double GasQuoteRate { get; set; }

        [JsonPropertyName("log_events")]
        public List<LogEvent> LogEvents { get; set; }
    }
}