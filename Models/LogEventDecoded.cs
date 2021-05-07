using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace safuCHARTS.Models
{
    public class LogEventDecoded
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("signature")]
        public string Signature { get; set; }

        [JsonPropertyName("params")]
        public List<LogEventDecodedParam> Params { get; set; }
    }
}