using Xeptions;

namespace Smart.Api.Models.Customer.Exceptions
{
    public class CustomersValidationException : Xeption
    {
        public CustomersValidationException(Xeption innerException)
            : base(message: "Customers validation errors occurred, please try again.",
                  innerException)
        { }
    }
}