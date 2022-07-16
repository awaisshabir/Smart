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
        public async Task ShouldRetrieveProductByIdAsync()
        {
            // given
            Product randomProduct = CreateRandomProduct();
            Product inputProduct = randomProduct;
            Product storageProduct = randomProduct;
            Product expectedProduct = storageProduct.DeepClone();

            this.storageBrokerMock.Setup(broker =>
                broker.SelectProductByIdAsync(inputProduct.Id))
                    .ReturnsAsync(storageProduct);

            // when
            Product actualProduct =
                await this.productService.RetrieveProductByIdAsync(inputProduct.Id);

            // then
            actualProduct.Should().BeEquivalentTo(expectedProduct);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectProductByIdAsync(inputProduct.Id),
                    Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}