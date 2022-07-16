using System.Threading.Tasks;
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
        public async Task ShouldThrowCriticalDependencyExceptionOnRemoveIfSqlErrorOccursAndLogItAsync()
        {
            // given
            Product randomProduct = CreateRandomProduct();
            SqlException sqlException = GetSqlException();

            var failedProductStorageException =
                new FailedProductStorageException(sqlException);

            var expectedProductDependencyException =
                new ProductDependencyException(failedProductStorageException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectProductByIdAsync(randomProduct.Id))
                    .Throws(sqlException);

            // when
            ValueTask<Product> addProductTask =
                this.productService.RemoveProductByIdAsync(randomProduct.Id);

            ProductDependencyException actualProductDependencyException =
                await Assert.ThrowsAsync<ProductDependencyException>(
                    addProductTask.AsTask);

            // then
            actualProductDependencyException.Should()
                .BeEquivalentTo(expectedProductDependencyException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectProductByIdAsync(randomProduct.Id),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogCritical(It.Is(SameExceptionAs(
                    expectedProductDependencyException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.DeleteProductAsync(It.IsAny<Product>()),
                    Times.Never);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffset(),
                    Times.Never);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }
    }
}