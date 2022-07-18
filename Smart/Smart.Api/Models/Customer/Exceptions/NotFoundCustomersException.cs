using System;
using Xeptions;

namespace Smart.Api.Models.Customer.Exceptions
{
    public class NotFoundCustomersException : Xeption
    {
        public NotFoundCustomersException(Guid customersId)
            : base(message: $"Couldn't find customers with customersId: {customersId}.")
        { }
    }
}