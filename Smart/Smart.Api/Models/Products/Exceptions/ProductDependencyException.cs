using Xeptions;

namespace Smart.Api.Models.Products.Exceptions
{
    public class ProductDependencyException : Xeption
    {
        public ProductDependencyException(Xeption innerException) :
            base(message: "Product dependency error occurred, contact support.", innerException)
        { }
    }
}