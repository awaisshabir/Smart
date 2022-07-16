using Xeptions;

namespace Smart.Api.Models.Customers.Exceptions
{
    public class CustomerDependencyValidationException : Xeption
    {
        public CustomerDependencyValidationException(Xeption innerException)
            : base(message: "Customer dependency validation occurred, please try again.", innerException)
        { }
    }
}