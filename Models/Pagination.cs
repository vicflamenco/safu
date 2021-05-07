using System.Text.Json.Serialization;

namespace safuCHARTS.Models
{
    public class Pagination
    {
        [JsonPropertyName("has_more")]
        public bool HasMore { get; set; }

        [JsonPropertyName("page_number")]
        public int PageNumber { get; set; }

        [JsonPropertyName("page_size")]
        public int PageSize { get; set; }

        [JsonPropertyName("total_count")]
        public int? TotalCount { get; set; }
    }
}