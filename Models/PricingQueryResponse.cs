using System.Text.Json.Serialization;

namespace safuCHARTS.Models
{
    public class PricingQueryResponse
    {
        [JsonPropertyName("data")]
        public PricingDataContent Data { get; set; }

        [JsonPropertyName("error")]
        public bool Error { get; set; }

        [JsonPropertyName("error_message")]
        public string ErrorMessage { get; set; }

        [JsonPropertyName("error_code")]
        public string ErrorCode { get; set; }
    }
}