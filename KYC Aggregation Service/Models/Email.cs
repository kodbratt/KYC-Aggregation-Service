using System.Text.Json.Serialization;

namespace KYC_Aggregation_Service.Models
{
    public class Email
    {
        [JsonPropertyName("preferred")]
        public bool Preferred { get; set; }

        [JsonPropertyName("emailAddress")]
        public string? EmailAddress { get; set; }
    }
}
