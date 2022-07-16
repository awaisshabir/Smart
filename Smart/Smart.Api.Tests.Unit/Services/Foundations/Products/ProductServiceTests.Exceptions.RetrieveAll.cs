using System;
using FluentAssertions;
using Microsoft.Data.SqlClient;
using Moq;
using Smart.Api.Models.Products.Exceptions;
using Xunit;

namespace Smart.Api.Tests.Unit.Services.Foundations.Products
{
    public partial class ProductServiceTests
    {
        [Fact]
        public void ShouldThrowCriticalDependencyExceptionOnRetrieveAllWhenSqlExceptionOccursAndLogIt()
        {
            // given
            SqlException sqlException = GetSqlException();

            var failedStorageException =
                new FailedProductStorageException(sqlException);

            var expectedProductDependencyException =
                new ProductDependencyException(failedStorageException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectAllProducts())
                    .Throws(sqlException);

            // when
            Action retrieveAllProductsAction = () =>
                this.productService.RetrieveAllProducts();

            ProductDependencyException actualProductDependencyException =
                Assert.Throws<ProductDependencyException>(retrieveAllProductsAction);

            // then
            actualProductDependencyException.Should()
                .BeEquivalentTo(expectedProductDependencyException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectAllProducts(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogCritical(It.Is(SameExceptionAs(
                    expectedProductDependencyException))),
                        Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }
    }
}