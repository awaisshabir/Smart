using System;
using Xeptions;

namespace Smart.Api.Models.Products.Exceptions
{
    public class InvalidProductReferenceException : Xeption
    {
        public InvalidProductReferenceException(Exception innerException)
            : base(message: "Invalid product reference error occurred.", innerException) { }
    }
}