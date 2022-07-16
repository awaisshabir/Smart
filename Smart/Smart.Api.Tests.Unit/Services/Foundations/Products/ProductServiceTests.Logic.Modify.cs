using System;
using System.Threading.Tasks;
using FluentAssertions;
using Force.DeepCloner;
using Moq;
using Smart.Api.Models.Products;
using Xunit;

namespace Smart.Api.Tests.Unit.Services.Foundations.Products
{
    public partial class ProductServiceTests
    {
        [Fact]
        public async Task ShouldModifyProductAsync()
        {
            // given
            DateTimeOffset randomDateTimeOffset = GetRandomDateTimeOffset();
            Product randomProduct = CreateRandomModifyProduct(randomDateTimeOffset);
            Product inputProduct = randomProduct;
            Product storageProduct = inputProduct.DeepClone();
            storageProduct.UpdatedDate = randomProduct.CreatedDate;
            Product updatedProduct = inputProduct;
            Product expectedProduct = updatedProduct.DeepClone();
            Guid productId = inputProduct.Id;

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffset())
                    .Returns(randomDateTimeOffset);

            this.storageBrokerMock.Setup(broker =>
                broker.UpdateProductAsync(inputProduct))
                    .ReturnsAsync(updatedProduct);

            // when
            Product actualProduct =
                await this.productService.ModifyProductAsync(inputProduct);

            // then
            actualProduct.Should().BeEquivalentTo(expectedProduct);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffset(),
                    Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.UpdateProductAsync(inputProduct),
                    Times.Once);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}