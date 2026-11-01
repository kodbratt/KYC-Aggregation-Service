namespace KYC_Aggregation_Service.Persistence.Entities
{
    public class AggregatedKycEntity
    {
        public int Id { get; set; }
        public string Ssn { get; set; } = string.Empty;
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Address { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Email { get; set; }
        public string? TaxCountry { get; set; }
        public int? Income { get; set; }
    }
}
