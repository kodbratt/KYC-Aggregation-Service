using System.Text.Json.Serialization;

namespace KYC_Aggregation_Service.Models
{
    public class ContactDetails
    {
        [JsonPropertyName("addresses")]
        public List<Address> Addresses { get; set; } = [];

        [JsonPropertyName("emails")]
        public List<Email> Emails { get; set; } = [];

        [JsonPropertyName("phoneNumbers")]
        public List<PhoneNumber> PhoneNumbers { get; set; } = [];
    }
}
