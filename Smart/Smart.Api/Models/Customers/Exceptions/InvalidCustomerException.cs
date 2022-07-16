using Xeptions;

namespace Smart.Api.Models.Customers.Exceptions
{
    public class InvalidCustomerException : Xeption
    {
        public InvalidCustomerException()
            : base(message: "Invalid customer. Please correct the errors and try again.")
        { }
    }
}