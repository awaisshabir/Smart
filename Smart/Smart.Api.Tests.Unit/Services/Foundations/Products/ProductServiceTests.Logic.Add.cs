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
        public async Task ShouldAddProductAsync()
        {
            // given
            DateTimeOffset randomDateTimeOffset =
                GetRandomDateTimeOffset();

            Product randomProduct = CreateRandomProduct(randomDateTimeOffset);
            Product inputProduct = randomProduct;
            Product storageProduct = inputProduct;
            Product expectedProduct = storageProduct.DeepClone();

            this.storageBrokerMock.Setup(broker =>
                broker.InsertProductAsync(inputProduct))
                    .ReturnsAsync(storageProduct);

            // when
            Product actualProduct = await this.productService
                .AddProductAsync(inputProduct);

            // then
            actualProduct.Should().BeEquivalentTo(expectedProduct);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertProductAsync(inputProduct),
                    Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }
    }
}