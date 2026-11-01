using System.Text.Json.Serialization;

namespace KYC_Aggregation_Service.Models
{
    public class KYCItem
    {
        [JsonPropertyName("key")]
        public string? Key { get; set; }

        [JsonPropertyName("value")]
        public string? Value { get; set; }
    }
}
