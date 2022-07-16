using System;
using Xeptions;

namespace Smart.Api.Models.Customers.Exceptions
{
    public class FailedCustomerServiceException : Xeption
    {
        public FailedCustomerServiceException(Exception innerException)
            : base(message: "Failed customer service occurred, please contact support", innerException)
        { }
    }
}