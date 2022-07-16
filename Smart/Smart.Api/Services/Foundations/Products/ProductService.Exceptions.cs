using System.Threading.Tasks;
using EFxceptions.Models.Exceptions;
using Microsoft.Data.SqlClient;
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
            catch (InvalidProductException invalidProductException)
            {
                throw CreateAndLogValidationException(invalidProductException);
            }
            catch (SqlException sqlException)
            {
                var failedProductStorageException =
                    new FailedProductStorageException(sqlException);

                throw CreateAndLogCriticalDependencyException(failedProductStorageException);
            }
            catch (DuplicateKeyException duplicateKeyException)
            {
                var alreadyExistsProductException =
                    new AlreadyExistsProductException(duplicateKeyException);

                throw CreateAndLogDependencyValidationException(alreadyExistsProductException);
            }
            catch (ForeignKeyConstraintConflictException foreignKeyConstraintConflictException)
            {
                var invalidProductReferenceException =
                    new InvalidProductReferenceException(foreignKeyConstraintConflictException);

                throw CreateAndLogDependencyValidationException(invalidProductReferenceException);
            }
        }

        private ProductValidationException CreateAndLogValidationException(Xeption exception)
        {
            var productValidationException =
                new ProductValidationException(exception);

            this.loggingBroker.LogError(productValidationException);

            return productValidationException;
        }

        private ProductDependencyException CreateAndLogCriticalDependencyException(Xeption exception)
        {
            var productDependencyException = new ProductDependencyException(exception);
            this.loggingBroker.LogCritical(productDependencyException);

            return productDependencyException;
        }

        private ProductDependencyValidationException CreateAndLogDependencyValidationException(Xeption exception)
        {
            var productDependencyValidationException =
                new ProductDependencyValidationException(exception);

            this.loggingBroker.LogError(productDependencyValidationException);

            return productDependencyValidationException;
        }
    }
}