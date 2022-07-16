using Xeptions;

namespace Smart.Api.Models.Customers.Exceptions
{
    public class CustomerDependencyException : Xeption
    {
        public CustomerDependencyException(Xeption innerException) :
            base(message: "Customer dependency error occurred, contact support.", innerException)
        { }
    }
}