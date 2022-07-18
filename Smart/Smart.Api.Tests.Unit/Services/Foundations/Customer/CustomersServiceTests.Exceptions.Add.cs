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
        public async Task ShouldThrowCriticalDependencyExceptionOnAddIfSqlErrorOccursAndLogItAsync()
        {
            // given
            Customers someCustomers = CreateRandomCustomers();
            SqlException sqlException = GetSqlException();

            var failedCustomersStorageException =
                new FailedCustomersStorageException(sqlException);

            var expectedCustomersDependencyException =
                new CustomersDependencyException(failedCustomersStorageException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffset())
                    .Throws(sqlException);

            // when
            ValueTask<Customers> addCustomersTask =
                this.customersService.AddCustomersAsync(someCustomers);

            CustomersDependencyException actualCustomersDependencyException =
                await Assert.ThrowsAsync<CustomersDependencyException>(
                    addCustomersTask.AsTask);

            // then
            actualCustomersDependencyException.Should()
                .BeEquivalentTo(expectedCustomersDependencyException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffset(),
                    Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertCustomersAsync(It.IsAny<Customers>()),
                    Times.Never);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogCritical(It.Is(SameExceptionAs(
                    expectedCustomersDependencyException))),
                        Times.Once);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}