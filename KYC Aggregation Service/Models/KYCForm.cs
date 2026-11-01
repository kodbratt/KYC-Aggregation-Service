using System.Text.Json.Serialization;

namespace KYC_Aggregation_Service.Models
{
    public class KYCForm
    {
        [JsonPropertyName("items")]
        public List<KYCItem> Items { get; set; } = [];
    }
}
