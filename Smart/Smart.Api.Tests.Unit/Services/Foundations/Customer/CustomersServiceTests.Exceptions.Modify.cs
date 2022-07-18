using System.Threading.Tasks;
using EFxceptions.Models.Exceptions;
using FluentAssertions;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Moq;
using Smart.Api.Models.Customer;
using Smart.Api.Models.Customer.Exceptions;
using Xunit;

namespace Smart.Api.Tests.Unit.Services.Foundations.Customer
{
    public partial class CustomersServiceTests
    {
        [Fact]
        public async Task ShouldThrowCriticalDependencyExceptionOnModifyIfSqlErrorOccursAndLogItAsync()
        {
            // given
            Customers randomCustomers = CreateRandomCustomers();
            SqlException sqlException = GetSqlException();

            var failedCustomersStorageException =
                new FailedCustomersStorageException(sqlException);

            var expectedCustomersDependencyException =
                new CustomersDependencyException(failedCustomersStorageException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffset())
                    .Throws(sqlException);

            // when
            ValueTask<Customers> modifyCustomersTask =
                this.customersService.ModifyCustomersAsync(randomCustomers);

            CustomersDependencyException actualCustomersDependencyException =
                await Assert.ThrowsAsync<CustomersDependencyException>(
                    modifyCustomersTask.AsTask);

            // then
            actualCustomersDependencyException.Should()
                .BeEquivalentTo(expectedCustomersDependencyException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffset(),
                    Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectCustomersByIdAsync(randomCustomers.Id),
                    Times.Never);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogCritical(It.Is(SameExceptionAs(
                    expectedCustomersDependencyException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.UpdateCustomersAsync(randomCustomers),
                    Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async void ShouldThrowValidationExceptionOnModifyIfReferenceErrorOccursAndLogItAsync()
        {
            // given
            Customers someCustomers = CreateRandomCustomers();
            string randomMessage = GetRandomMessage();
            string exceptionMessage = randomMessage;

            var foreignKeyConstraintConflictException =
                new ForeignKeyConstraintConflictException(exceptionMessage);

            var invalidCustomersReferenceException =
                new InvalidCustomersReferenceException(foreignKeyConstraintConflictException);

            CustomersDependencyValidationException expectedCustomersDependencyValidationException =
                new CustomersDependencyValidationException(invalidCustomersReferenceException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffset())
                    .Throws(foreignKeyConstraintConflictException);

            // when
            ValueTask<Customers> modifyCustomersTask =
                this.customersService.ModifyCustomersAsync(someCustomers);

            CustomersDependencyValidationException actualCustomersDependencyValidationException =
                await Assert.ThrowsAsync<CustomersDependencyValidationException>(
                    modifyCustomersTask.AsTask);

            // then
            actualCustomersDependencyValidationException.Should()
                .BeEquivalentTo(expectedCustomersDependencyValidationException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffset(),
                    Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectCustomersByIdAsync(someCustomers.Id),
                    Times.Never);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(expectedCustomersDependencyValidationException))),
                    Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.UpdateCustomersAsync(someCustomers),
                    Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowDependencyExceptionOnModifyIfDatabaseUpdateExceptionOccursAndLogItAsync()
        {
            // given
            Customers randomCustomers = CreateRandomCustomers();
            var databaseUpdateException = new DbUpdateException();

            var failedCustomersStorageException =
                new FailedCustomersStorageException(databaseUpdateException);

            var expectedCustomersDependencyException =
                new CustomersDependencyException(failedCustomersStorageException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffset())
                    .Throws(databaseUpdateException);

            // when
            ValueTask<Customers> modifyCustomersTask =
                this.customersService.ModifyCustomersAsync(randomCustomers);

            CustomersDependencyException actualCustomersDependencyException =
                await Assert.ThrowsAsync<CustomersDependencyException>(
                    modifyCustomersTask.AsTask);

            // then
            actualCustomersDependencyException.Should()
                .BeEquivalentTo(expectedCustomersDependencyException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffset(),
                    Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectCustomersByIdAsync(randomCustomers.Id),
                    Times.Never);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedCustomersDependencyException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.UpdateCustomersAsync(randomCustomers),
                    Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}