using System;
using Xeptions;

namespace Smart.Api.Models.Customer.Exceptions
{
    public class CustomersServiceException : Xeption
    {
        public CustomersServiceException(Exception innerException)
            : base(message: "Customers service error occurred, contact support.", innerException)
        { }
    }
}