using System.Text.Json.Serialization;

namespace KYC_Aggregation_Service.Models
{
    public class PersonalDetails
    {
        [JsonPropertyName("firstName")]
        public string? FirstName { get; set; }

        [JsonPropertyName("surName")]
        public string? SurName { get; set; }

        [JsonPropertyName("address")]
        public string? Address { get; set; }
    }
}
