using System.Threading.Tasks;
using EFxceptions.Models.Exceptions;
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

        [Fact]
        public async Task ShouldThrowDependencyValidationExceptionOnAddIfCustomersAlreadyExsitsAndLogItAsync()
        {
            // given
            Customers randomCustomers = CreateRandomCustomers();
            Customers alreadyExistsCustomers = randomCustomers;
            string randomMessage = GetRandomMessage();

            var duplicateKeyException =
                new DuplicateKeyException(randomMessage);

            var alreadyExistsCustomersException =
                new AlreadyExistsCustomersException(duplicateKeyException);

            var expectedCustomersDependencyValidationException =
                new CustomersDependencyValidationException(alreadyExistsCustomersException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffset())
                    .Throws(duplicateKeyException);

            // when
            ValueTask<Customers> addCustomersTask =
                this.customersService.AddCustomersAsync(alreadyExistsCustomers);

            // then
            CustomersDependencyValidationException actualCustomersDependencyValidationException =
                await Assert.ThrowsAsync<CustomersDependencyValidationException>(
                    addCustomersTask.AsTask);

            actualCustomersDependencyValidationException.Should()
                .BeEquivalentTo(expectedCustomersDependencyValidationException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffset(),
                    Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertCustomersAsync(It.IsAny<Customers>()),
                    Times.Never);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedCustomersDependencyValidationException))),
                        Times.Once);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async void ShouldThrowValidationExceptionOnAddIfReferenceErrorOccursAndLogItAsync()
        {
            // given
            Customers someCustomers = CreateRandomCustomers();
            string randomMessage = GetRandomMessage();
            string exceptionMessage = randomMessage;

            var foreignKeyConstraintConflictException =
                new ForeignKeyConstraintConflictException(exceptionMessage);

            var invalidCustomersReferenceException =
                new InvalidCustomersReferenceException(foreignKeyConstraintConflictException);

            var expectedCustomersValidationException =
                new CustomersDependencyValidationException(invalidCustomersReferenceException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffset())
                    .Throws(foreignKeyConstraintConflictException);

            // when
            ValueTask<Customers> addCustomersTask =
                this.customersService.AddCustomersAsync(someCustomers);

            // then
            CustomersDependencyValidationException actualCustomersDependencyValidationException =
                await Assert.ThrowsAsync<CustomersDependencyValidationException>(
                    addCustomersTask.AsTask);

            actualCustomersDependencyValidationException.Should().BeEquivalentTo(expectedCustomersValidationException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffset(),
                    Times.Once());

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedCustomersValidationException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertCustomersAsync(someCustomers),
                    Times.Never());

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }
    }
}