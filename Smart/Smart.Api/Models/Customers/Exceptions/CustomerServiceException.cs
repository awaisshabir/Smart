using System;
using Xeptions;

namespace Smart.Api.Models.Customers.Exceptions
{
    public class CustomerServiceException : Xeption
    {
        public CustomerServiceException(Exception innerException)
            : base(message: "Customer service error occurred, contact support.", innerException)
        { }
    }
}