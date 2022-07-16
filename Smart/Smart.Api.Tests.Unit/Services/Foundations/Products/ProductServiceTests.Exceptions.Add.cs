using System.Threading.Tasks;
using EFxceptions.Models.Exceptions;
using FluentAssertions;
using Microsoft.Data.SqlClient;
using Moq;
using Smart.Api.Models.Products;
using Smart.Api.Models.Products.Exceptions;
using Xunit;

namespace Smart.Api.Tests.Unit.Services.Foundations.Products
{
    public partial class ProductServiceTests
    {
        [Fact]
        public async Task ShouldThrowCriticalDependencyExceptionOnAddIfSqlErrorOccursAndLogItAsync()
        {
            // given
            Product someProduct = CreateRandomProduct();
            SqlException sqlException = GetSqlException();

            var failedProductStorageException =
                new FailedProductStorageException(sqlException);

            var expectedProductDependencyException =
                new ProductDependencyException(failedProductStorageException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffset())
                    .Throws(sqlException);

            // when
            ValueTask<Product> addProductTask =
                this.productService.AddProductAsync(someProduct);

            ProductDependencyException actualProductDependencyException =
                await Assert.ThrowsAsync<ProductDependencyException>(
                    addProductTask.AsTask);

            // then
            actualProductDependencyException.Should()
                .BeEquivalentTo(expectedProductDependencyException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffset(),
                    Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertProductAsync(It.IsAny<Product>()),
                    Times.Never);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogCritical(It.Is(SameExceptionAs(
                    expectedProductDependencyException))),
                        Times.Once);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowDependencyValidationExceptionOnAddIfProductAlreadyExsitsAndLogItAsync()
        {
            // given
            Product randomProduct = CreateRandomProduct();
            Product alreadyExistsProduct = randomProduct;
            string randomMessage = GetRandomMessage();

            var duplicateKeyException =
                new DuplicateKeyException(randomMessage);

            var alreadyExistsProductException =
                new AlreadyExistsProductException(duplicateKeyException);

            var expectedProductDependencyValidationException =
                new ProductDependencyValidationException(alreadyExistsProductException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffset())
                    .Throws(duplicateKeyException);

            // when
            ValueTask<Product> addProductTask =
                this.productService.AddProductAsync(alreadyExistsProduct);

            // then
            ProductDependencyValidationException actualProductDependencyValidationException =
                await Assert.ThrowsAsync<ProductDependencyValidationException>(
                    addProductTask.AsTask);

            actualProductDependencyValidationException.Should()
                .BeEquivalentTo(expectedProductDependencyValidationException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffset(),
                    Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertProductAsync(It.IsAny<Product>()),
                    Times.Never);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedProductDependencyValidationException))),
                        Times.Once);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}