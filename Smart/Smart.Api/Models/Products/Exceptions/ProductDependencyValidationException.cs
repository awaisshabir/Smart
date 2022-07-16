using Xeptions;

namespace Smart.Api.Models.Products.Exceptions
{
    public class ProductDependencyValidationException : Xeption
    {
        public ProductDependencyValidationException(Xeption innerException)
            : base(message: "Product dependency validation occurred, please try again.", innerException)
        { }
    }
}