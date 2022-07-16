using Xeptions;

namespace Smart.Api.Models.Customers.Exceptions
{
    public class CustomerValidationException : Xeption
    {
        public CustomerValidationException(Xeption innerException)
            : base(message: "Customer validation errors occurred, please try again.",
                  innerException)
        { }
    }
}