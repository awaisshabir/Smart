using Xeptions;

namespace Smart.Api.Models.Products.Exceptions
{
    public class InvalidProductException : Xeption
    {
        public InvalidProductException()
            : base(message: "Invalid product. Please correct the errors and try again.")
        { }
    }
}