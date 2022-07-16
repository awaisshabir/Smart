using Xeptions;

namespace Smart.Api.Models.Customers.Exceptions
{
    public class NullCustomerException : Xeption
    {
        public NullCustomerException()
            : base(message: "Customer is null.")
        { }
    }
}