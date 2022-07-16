using System;
using Xeptions;

namespace Smart.Api.Models.Products.Exceptions
{
    public class LockedProductException : Xeption
    {
        public LockedProductException(Exception innerException)
            : base(message: "Locked product record exception, please try again later", innerException)
        {
        }
    }
}