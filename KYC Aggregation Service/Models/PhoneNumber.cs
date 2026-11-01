using System.Text.Json.Serialization;

namespace KYC_Aggregation_Service.Models
{
    public class PhoneNumber
    {
        [JsonPropertyName("preferred")]
        public bool Preferred { get; set; }

        [JsonPropertyName("number")]
        public string? Number { get; set; }
    }
}
