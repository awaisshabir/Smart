using Xeptions;

namespace Smart.Api.Models.Products.Exceptions
{
    public class NullProductException : Xeption
    {
        public NullProductException()
            : base(message: "Product is null.")
        { }
    }
}