using Xeptions;

namespace Smart.Api.Models.Customer.Exceptions
{
    public class CustomersDependencyValidationException : Xeption
    {
        public CustomersDependencyValidationException(Xeption innerException)
            : base(message: "Customers dependency validation occurred, please try again.", innerException)
        { }
    }
}