namespace KYC_Aggregation_Service.Exceptions
{
    public sealed class CustomerNotFoundException : Exception
    {
        public CustomerNotFoundException()
            : base("Customer data not found for the provided SSN.")
        {
        }

        public CustomerNotFoundException(Exception innerException)
            : base(
                "Customer data not found for the provided SSN.",
                innerException)
        {
        }
    }
}
