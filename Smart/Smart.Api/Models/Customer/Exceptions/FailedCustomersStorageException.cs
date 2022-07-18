using System;
using Xeptions;

namespace Smart.Api.Models.Customer.Exceptions
{
    public class FailedCustomersStorageException : Xeption
    {
        public FailedCustomersStorageException(Exception innerException)
            : base(message: "Failed customers storage error occurred, contact support.", innerException)
        { }
    }
}