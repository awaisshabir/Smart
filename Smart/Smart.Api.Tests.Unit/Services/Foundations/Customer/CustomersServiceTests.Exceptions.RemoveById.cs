using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Data.SqlClient;
using Moq;
using Smart.Api.Models.Customer;
using Smart.Api.Models.Customer.Exceptions;
using Xunit;

namespace Smart.Api.Tests.Unit.Services.Foundations.Customer
{
    public partial class CustomersServiceTests
    {
        [Fact]
        public async Task ShouldThrowCriticalDependencyExceptionOnRemoveIfSqlErrorOccursAndLogItAsync()
        {
            // given
            Customers randomCustomers = CreateRandomCustomers();
            SqlException sqlException = GetSqlException();

            var failedCustomersStorageException =
                new FailedCustomersStorageException(sqlException);

            var expectedCustomersDependencyException =
                new CustomersDependencyException(failedCustomersStorageException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectCustomersByIdAsync(randomCustomers.Id))
                    .Throws(sqlException);

            // when
            ValueTask<Customers> addCustomersTask =
                this.customersService.RemoveCustomersByIdAsync(randomCustomers.Id);

            CustomersDependencyException actualCustomersDependencyException =
                await Assert.ThrowsAsync<CustomersDependencyException>(
                    addCustomersTask.AsTask);

            // then
            actualCustomersDependencyException.Should()
                .BeEquivalentTo(expectedCustomersDependencyException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectCustomersByIdAsync(randomCustomers.Id),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogCritical(It.Is(SameExceptionAs(
                    expectedCustomersDependencyException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.DeleteCustomersAsync(It.IsAny<Customers>()),
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