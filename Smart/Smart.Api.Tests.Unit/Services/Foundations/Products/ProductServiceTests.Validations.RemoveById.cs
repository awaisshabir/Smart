using System;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using Smart.Api.Models.Products;
using Smart.Api.Models.Products.Exceptions;
using Xunit;

namespace Smart.Api.Tests.Unit.Services.Foundations.Products
{
    public partial class ProductServiceTests
    {
        [Fact]
        public async Task ShouldThrowValidationExceptionOnRemoveIfIdIsInvalidAndLogItAsync()
        {
            // given
            Guid invalidProductId = Guid.Empty;

            var invalidProductException =
                new InvalidProductException();

            invalidProductException.AddData(
                key: nameof(Product.Id),
                values: "Id is required");

            var expectedProductValidationException =
                new ProductValidationException(invalidProductException);

            // when
            ValueTask<Product> removeProductByIdTask =
                this.productService.RemoveProductByIdAsync(invalidProductId);

            ProductValidationException actualProductValidationException =
                await Assert.ThrowsAsync<ProductValidationException>(
                    removeProductByIdTask.AsTask);

            // then
            actualProductValidationException.Should()
                .BeEquivalentTo(expectedProductValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedProductValidationException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.DeleteProductAsync(It.IsAny<Product>()),
                    Times.Never);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }
    }
}