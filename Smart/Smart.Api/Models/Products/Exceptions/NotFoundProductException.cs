using System;
using Xeptions;

namespace Smart.Api.Models.Products.Exceptions
{
    public class NotFoundProductException : Xeption
    {
        public NotFoundProductException(Guid productId)
            : base(message: $"Couldn't find product with productId: {productId}.")
        { }
    }
}