using Xeptions;

namespace Smart.Api.Models.Products.Exceptions
{
    public class ProductValidationException : Xeption
    {
        public ProductValidationException(Xeption innerException)
            : base(message: "Product validation errors occurred, please try again.",
                  innerException)
        { }
    }
}