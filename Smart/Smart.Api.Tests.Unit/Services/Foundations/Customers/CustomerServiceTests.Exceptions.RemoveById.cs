using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Data.SqlClient;
using Moq;
using Smart.Api.Models.Customers;
using Smart.Api.Models.Customers.Exceptions;
using Xunit;

namespace Smart.Api.Tests.Unit.Services.Foundations.Customers
{
    public partial class CustomerServiceTests
    {
        [Fact]
        public async Task ShouldThrowCriticalDependencyExceptionOnRemoveIfSqlErrorOccursAndLogItAsync()
        {
            // given
            Customer randomCustomer = CreateRandomCustomer();
            SqlException sqlException = GetSqlException();

            var failedCustomerStorageException =
                new FailedCustomerStorageException(sqlException);

            var expectedCustomerDependencyException =
                new CustomerDependencyException(failedCustomerStorageException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectCustomerByIdAsync(randomCustomer.Id))
                    .Throws(sqlException);

            // when
            ValueTask<Customer> addCustomerTask =
                this.customerService.RemoveCustomerByIdAsync(randomCustomer.Id);

            CustomerDependencyException actualCustomerDependencyException =
                await Assert.ThrowsAsync<CustomerDependencyException>(
                    addCustomerTask.AsTask);

            // then
            actualCustomerDependencyException.Should()
                .BeEquivalentTo(expectedCustomerDependencyException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectCustomerByIdAsync(randomCustomer.Id),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogCritical(It.Is(SameExceptionAs(
                    expectedCustomerDependencyException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.DeleteCustomerAsync(It.IsAny<Customer>()),
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