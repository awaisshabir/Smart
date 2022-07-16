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
        public async Task ShouldThrowValidationExceptionOnAddIfProductIsNullAndLogItAsync()
        {
            // given
            Product nullProduct = null;

            var nullProductException =
                new NullProductException();

            var expectedProductValidationException =
                new ProductValidationException(nullProductException);

            // when
            ValueTask<Product> addProductTask =
                this.productService.AddProductAsync(nullProduct);

            ProductValidationException actualProductValidationException =
                await Assert.ThrowsAsync<ProductValidationException>(
                    addProductTask.AsTask);

            // then
            actualProductValidationException.Should().BeEquivalentTo(expectedProductValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedProductValidationException))),
                        Times.Once);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }
    }
}