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
            actualProductValidationException.Should()
                .BeEquivalentTo(expectedProductValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedProductValidationException))),
                        Times.Once);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public async Task ShouldThrowValidationExceptionOnAddIfProductIsInvalidAndLogItAsync(string invalidText)
        {
            // given
            var invalidProduct = new Product
            {
                // Name = invalidText
            };

            var invalidProductException =
                new InvalidProductException();

            invalidProductException.AddData(
                key: nameof(Product.Id),
                values: "Id is required");

            //invalidProductException.AddData(
            //    key: nameof(Product.Name),
            //    values: "Text is required");

            // TODO: Add or remove data here to suit the validation needs for the Product model

            invalidProductException.AddData(
                key: nameof(Product.CreatedDate),
                values: "Date is required");

            invalidProductException.AddData(
                key: nameof(Product.CreatedByUserId),
                values: "Id is required");

            invalidProductException.AddData(
                key: nameof(Product.UpdatedDate),
                values: "Date is required");

            invalidProductException.AddData(
                key: nameof(Product.UpdatedByUserId),
                values: "Id is required");

            var expectedProductValidationException =
                new ProductValidationException(invalidProductException);

            // when
            ValueTask<Product> addProductTask =
                this.productService.AddProductAsync(invalidProduct);

            ProductValidationException actualProductValidationException =
                await Assert.ThrowsAsync<ProductValidationException>(
                    addProductTask.AsTask);

            // then
            actualProductValidationException.Should()
                .BeEquivalentTo(expectedProductValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedProductValidationException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertProductAsync(It.IsAny<Product>()),
                    Times.Never);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowValidationExceptionOnAddIfCreateAndUpdateDatesIsNotSameAndLogItAsync()
        {
            // given
            int randomNumber = GetRandomNumber();
            DateTimeOffset randomDateTimeOffset = GetRandomDateTimeOffset();
            Product randomProduct = CreateRandomProduct(randomDateTimeOffset);
            Product invalidProduct = randomProduct;

            invalidProduct.UpdatedDate =
                invalidProduct.CreatedDate.AddDays(randomNumber);

            var invalidProductException = new InvalidProductException();

            invalidProductException.AddData(
                key: nameof(Product.UpdatedDate),
                values: $"Date is not the same as {nameof(Product.CreatedDate)}");

            var expectedProductValidationException =
                new ProductValidationException(invalidProductException);

            // when
            ValueTask<Product> addProductTask =
                this.productService.AddProductAsync(invalidProduct);

            ProductValidationException actualProductValidationException =
                await Assert.ThrowsAsync<ProductValidationException>(
                    addProductTask.AsTask);

            // then
            actualProductValidationException.Should()
                .BeEquivalentTo(expectedProductValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedProductValidationException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertProductAsync(It.IsAny<Product>()),
                    Times.Never);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowValidationExceptionOnAddIfCreateAndUpdateUserIdsIsNotSameAndLogItAsync()
        {
            // given
            DateTimeOffset randomDateTimeOffset = GetRandomDateTimeOffset();
            Product randomProduct = CreateRandomProduct(randomDateTimeOffset);
            Product invalidProduct = randomProduct;
            invalidProduct.UpdatedByUserId = Guid.NewGuid();

            var invalidProductException =
                new InvalidProductException();

            invalidProductException.AddData(
                key: nameof(Product.UpdatedByUserId),
                values: $"Id is not the same as {nameof(Product.CreatedByUserId)}");

            var expectedProductValidationException =
                new ProductValidationException(invalidProductException);

            // when
            ValueTask<Product> addProductTask =
                this.productService.AddProductAsync(invalidProduct);

            ProductValidationException actualProductValidationException =
                await Assert.ThrowsAsync<ProductValidationException>(
                    addProductTask.AsTask);

            // then
            actualProductValidationException.Should()
                .BeEquivalentTo(expectedProductValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedProductValidationException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertProductAsync(It.IsAny<Product>()),
                    Times.Never);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }
    }
}