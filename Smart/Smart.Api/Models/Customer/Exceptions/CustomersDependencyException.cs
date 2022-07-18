using Xeptions;

namespace Smart.Api.Models.Customer.Exceptions
{
    public class CustomersDependencyException : Xeption
    {
        public CustomersDependencyException(Xeption innerException) :
            base(message: "Customers dependency error occurred, contact support.", innerException)
        { }
    }
}