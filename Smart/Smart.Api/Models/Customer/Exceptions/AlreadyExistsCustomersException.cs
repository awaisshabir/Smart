using System;
using Xeptions;

namespace Smart.Api.Models.Customer.Exceptions
{
    public class AlreadyExistsCustomersException : Xeption
    {
        public AlreadyExistsCustomersException(Exception innerException)
            : base(message: "Customers with the same Id already exists.", innerException)
        { }
    }
}