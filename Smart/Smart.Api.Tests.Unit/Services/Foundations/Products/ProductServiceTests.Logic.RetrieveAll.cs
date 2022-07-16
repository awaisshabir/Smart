using System.Linq;
using FluentAssertions;
using Moq;
using Smart.Api.Models.Products;
using Xunit;

namespace Smart.Api.Tests.Unit.Services.Foundations.Products
{
    public partial class ProductServiceTests
    {
        [Fact]
        public void ShouldReturnProducts()
        {
            // given
            IQueryable<Product> randomProducts = CreateRandomProducts();
            IQueryable<Product> storageProducts = randomProducts;
            IQueryable<Product> expectedProducts = storageProducts;

            this.storageBrokerMock.Setup(broker =>
                broker.SelectAllProducts())
                    .Returns(storageProducts);

            // when
            IQueryable<Product> actualProducts =
                this.productService.RetrieveAllProducts();

            // then
            actualProducts.Should().BeEquivalentTo(expectedProducts);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectAllProducts(),
                    Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}