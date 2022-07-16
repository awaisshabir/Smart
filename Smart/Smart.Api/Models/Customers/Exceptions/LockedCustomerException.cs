using System;
using Xeptions;

namespace Smart.Api.Models.Customers.Exceptions
{
    public class LockedCustomerException : Xeption
    {
        public LockedCustomerException(Exception innerException)
            : base(message: "Locked customer record exception, please try again later", innerException)
        {
        }
    }
}