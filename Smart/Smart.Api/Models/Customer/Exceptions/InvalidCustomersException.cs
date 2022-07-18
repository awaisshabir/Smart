using Xeptions;

namespace Smart.Api.Models.Customer.Exceptions
{
    public class InvalidCustomersException : Xeption
    {
        public InvalidCustomersException()
            : base(message: "Invalid customers. Please correct the errors and try again.")
        { }
    }
}