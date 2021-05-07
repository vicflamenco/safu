using System.Text.Json.Serialization;

namespace safuCHARTS.Models
{
    public class LogEvent
    {
        [JsonPropertyName("block_signed_at")]
        public string BlockSignedAt { get; set; }

        [JsonPropertyName("block_height")]
        public double BlockHeight { get; set; }

        [JsonPropertyName("tx_offset")]
        public int TxOffset { get; set; }

        [JsonPropertyName("log_offset")]
        public int LogOffset { get; set; }

        [JsonPropertyName("tx_hash")]
        public string TxHash { get; set; }

        [JsonPropertyName("sender_address")]
        public string SenderAddress { get; set; }

        [JsonPropertyName("sender_address_label")]
        public string SenderAddressLabel { get; set; }

        [JsonPropertyName("decoded")]
        public LogEventDecoded Decoded { get; set; }
    }
}