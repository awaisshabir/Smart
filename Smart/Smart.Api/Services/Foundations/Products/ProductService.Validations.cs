using Smart.Api.Models.Products;
using Smart.Api.Models.Products.Exceptions;

namespace Smart.Api.Services.Foundations.Products
{
    public partial class ProductService
    {
        private void ValidateProductOnAdd(Product product)
        {
            ValidateProductIsNotNull(product);
        }

        private static void ValidateProductIsNotNull(Product product)
        {
            if (product is null)
            {
                throw new NullProductException();
            }
        }
    }
}