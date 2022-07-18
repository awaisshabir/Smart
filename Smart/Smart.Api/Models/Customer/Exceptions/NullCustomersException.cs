using Xeptions;

namespace Smart.Api.Models.Customer.Exceptions
{
    public class NullCustomersException : Xeption
    {
        public NullCustomersException()
            : base(message: "Customers is null.")
        { }
    }
}