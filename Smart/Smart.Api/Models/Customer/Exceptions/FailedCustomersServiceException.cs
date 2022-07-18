using System;
using Xeptions;

namespace Smart.Api.Models.Customer.Exceptions
{
    public class FailedCustomersServiceException : Xeption
    {
        public FailedCustomersServiceException(Exception innerException)
            : base(message: "Failed customers service occurred, please contact support", innerException)
        { }
    }
}