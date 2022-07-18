using System;
using Xeptions;

namespace Smart.Api.Models.Customer.Exceptions
{
    public class LockedCustomersException : Xeption
    {
        public LockedCustomersException(Exception innerException)
            : base(message: "Locked customers record exception, please try again later", innerException)
        {
        }
    }
}