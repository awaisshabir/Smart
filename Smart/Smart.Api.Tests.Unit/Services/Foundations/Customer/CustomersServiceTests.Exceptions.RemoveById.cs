using System;
using System.Threading.Tasks;
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

        [Fact]
        public async Task ShouldThrowDependencyValidationOnRemoveIfDatabaseUpdateConcurrencyErrorOccursAndLogItAsync()
        {
            // given
            Guid someCustomersId = Guid.NewGuid();

            var databaseUpdateConcurrencyException =
                new DbUpdateConcurrencyException();

            var lockedCustomersException =
                new LockedCustomersException(databaseUpdateConcurrencyException);

            var expectedCustomersDependencyValidationException =
                new CustomersDependencyValidationException(lockedCustomersException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectCustomersByIdAsync(It.IsAny<Guid>()))
                    .ThrowsAsync(databaseUpdateConcurrencyException);

            // when
            ValueTask<Customers> removeCustomersByIdTask =
                this.customersService.RemoveCustomersByIdAsync(someCustomersId);

            CustomersDependencyValidationException actualCustomersDependencyValidationException =
                await Assert.ThrowsAsync<CustomersDependencyValidationException>(
                    removeCustomersByIdTask.AsTask);

            // then
            actualCustomersDependencyValidationException.Should()
                .BeEquivalentTo(expectedCustomersDependencyValidationException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectCustomersByIdAsync(It.IsAny<Guid>()),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedCustomersDependencyValidationException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.DeleteCustomersAsync(It.IsAny<Customers>()),
                    Times.Never);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowDependencyExceptionOnRemoveWhenSqlExceptionOccursAndLogItAsync()
        {
            // given
            Guid someCustomersId = Guid.NewGuid();
            SqlException sqlException = GetSqlException();

            var failedCustomersStorageException =
                new FailedCustomersStorageException(sqlException);

            var expectedCustomersDependencyException =
                new CustomersDependencyException(failedCustomersStorageException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectCustomersByIdAsync(It.IsAny<Guid>()))
                    .ThrowsAsync(sqlException);

            // when
            ValueTask<Customers> deleteCustomersTask =
                this.customersService.RemoveCustomersByIdAsync(someCustomersId);

            CustomersDependencyException actualCustomersDependencyException =
                await Assert.ThrowsAsync<CustomersDependencyException>(
                    deleteCustomersTask.AsTask);

            // then
            actualCustomersDependencyException.Should()
                .BeEquivalentTo(expectedCustomersDependencyException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectCustomersByIdAsync(It.IsAny<Guid>()),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogCritical(It.Is(SameExceptionAs(
                    expectedCustomersDependencyException))),
                        Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowServiceExceptionOnRemoveIfServiceErrorOccursAndLogItAsync()
        {
            // given
            Guid someCustomersId = Guid.NewGuid();
            var serviceException = new Exception();

            var failedCustomersServiceException =
                new FailedCustomersServiceException(serviceException);

            var expectedCustomersServiceException =
                new CustomersServiceException(failedCustomersServiceException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectCustomersByIdAsync(It.IsAny<Guid>()))
                    .ThrowsAsync(serviceException);

            // when
            ValueTask<Customers> removeCustomersByIdTask =
                this.customersService.RemoveCustomersByIdAsync(someCustomersId);

            CustomersServiceException actualCustomersServiceException =
                await Assert.ThrowsAsync<CustomersServiceException>(
                    removeCustomersByIdTask.AsTask);

            // then
            actualCustomersServiceException.Should()
                .BeEquivalentTo(expectedCustomersServiceException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectCustomersByIdAsync(It.IsAny<Guid>()),
                        Times.Once());

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedCustomersServiceException))),
                        Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }
    }
}