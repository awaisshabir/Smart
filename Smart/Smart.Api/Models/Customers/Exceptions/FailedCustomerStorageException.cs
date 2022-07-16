using System;
using Xeptions;

namespace Smart.Api.Models.Customers.Exceptions
{
    public class FailedCustomerStorageException : Xeption
    {
        public FailedCustomerStorageException(Exception innerException)
            : base(message: "Failed customer storage error occurred, contact support.", innerException)
        { }
    }
}