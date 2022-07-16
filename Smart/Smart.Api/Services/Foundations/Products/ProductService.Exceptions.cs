using System.Threading.Tasks;
using Smart.Api.Models.Products;
using Smart.Api.Models.Products.Exceptions;
using Xeptions;

namespace Smart.Api.Services.Foundations.Products
{
    public partial class ProductService
    {
        private delegate ValueTask<Product> ReturningProductFunction();

        private async ValueTask<Product> TryCatch(ReturningProductFunction returningProductFunction)
        {
            try
            {
                return await returningProductFunction();
            }
            catch (NullProductException nullProductException)
            {
                throw CreateAndLogValidationException(nullProductException);
            }
        }

        private ProductValidationException CreateAndLogValidationException(Xeption exception)
        {
            var productValidationException =
                new ProductValidationException(exception);

            this.loggingBroker.LogError(productValidationException);

            return productValidationException;
        }
    }
}