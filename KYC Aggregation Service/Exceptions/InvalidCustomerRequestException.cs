namespace KYC_Aggregation_Service.Exceptions;

public sealed class InvalidCustomerRequestException : Exception
{
    public InvalidCustomerRequestException()
        : base("The request sent to the Customer Data API was invalid.")
    {
    }

    public InvalidCustomerRequestException(Exception innerException)
        : base(
            "The request sent to the Customer Data API was invalid.",
            innerException)
    {
    }
}
