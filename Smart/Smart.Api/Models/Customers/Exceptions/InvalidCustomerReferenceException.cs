using System;
using Xeptions;

namespace Smart.Api.Models.Customers.Exceptions
{
    public class InvalidCustomerReferenceException : Xeption
    {
        public InvalidCustomerReferenceException(Exception innerException)
            : base(message: "Invalid customer reference error occurred.", innerException) { }
    }
}