using System;
using Xeptions;

namespace Smart.Api.Models.Customers.Exceptions
{
    public class AlreadyExistsCustomerException : Xeption
    {
        public AlreadyExistsCustomerException(Exception innerException)
            : base(message: "Customer with the same Id already exists.", innerException)
        { }
    }
}