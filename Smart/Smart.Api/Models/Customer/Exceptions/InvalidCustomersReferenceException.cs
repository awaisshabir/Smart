using System;
using Xeptions;

namespace Smart.Api.Models.Customer.Exceptions
{
    public class InvalidCustomersReferenceException : Xeption
    {
        public InvalidCustomersReferenceException(Exception innerException)
            : base(message: "Invalid customers reference error occurred.", innerException) { }
    }
}