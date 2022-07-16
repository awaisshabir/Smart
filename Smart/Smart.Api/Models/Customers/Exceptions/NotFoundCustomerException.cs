using System;
using Xeptions;

namespace Smart.Api.Models.Customers.Exceptions
{
    public class NotFoundCustomerException : Xeption
    {
        public NotFoundCustomerException(Guid customerId)
            : base(message: $"Couldn't find customer with customerId: {customerId}.")
        { }
    }
}