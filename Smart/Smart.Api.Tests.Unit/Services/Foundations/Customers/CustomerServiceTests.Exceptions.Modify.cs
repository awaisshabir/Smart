using System.Threading.Tasks;
using EFxceptions.Models.Exceptions;
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
        public async Task ShouldThrowCriticalDependencyExceptionOnModifyIfSqlErrorOccursAndLogItAsync()
        {
            // given
            Customer randomCustomer = CreateRandomCustomer();
            SqlException sqlException = GetSqlException();

            var failedCustomerStorageException =
                new FailedCustomerStorageException(sqlException);

            var expectedCustomerDependencyException =
                new CustomerDependencyException(failedCustomerStorageException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffset())
                    .Throws(sqlException);

            // when
            ValueTask<Customer> modifyCustomerTask =
                this.customerService.ModifyCustomerAsync(randomCustomer);

            CustomerDependencyException actualCustomerDependencyException =
                await Assert.ThrowsAsync<CustomerDependencyException>(
                    modifyCustomerTask.AsTask);

            // then
            actualCustomerDependencyException.Should()
                .BeEquivalentTo(expectedCustomerDependencyException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffset(),
                    Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectCustomerByIdAsync(randomCustomer.Id),
                    Times.Never);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogCritical(It.Is(SameExceptionAs(
                    expectedCustomerDependencyException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.UpdateCustomerAsync(randomCustomer),
                    Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async void ShouldThrowValidationExceptionOnModifyIfReferenceErrorOccursAndLogItAsync()
        {
            // given
            Customer someCustomer = CreateRandomCustomer();
            string randomMessage = GetRandomMessage();
            string exceptionMessage = randomMessage;

            var foreignKeyConstraintConflictException =
                new ForeignKeyConstraintConflictException(exceptionMessage);

            var invalidCustomerReferenceException =
                new InvalidCustomerReferenceException(foreignKeyConstraintConflictException);

            CustomerDependencyValidationException expectedCustomerDependencyValidationException =
                new CustomerDependencyValidationException(invalidCustomerReferenceException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffset())
                    .Throws(foreignKeyConstraintConflictException);

            // when
            ValueTask<Customer> modifyCustomerTask =
                this.customerService.ModifyCustomerAsync(someCustomer);

            CustomerDependencyValidationException actualCustomerDependencyValidationException =
                await Assert.ThrowsAsync<CustomerDependencyValidationException>(
                    modifyCustomerTask.AsTask);

            // then
            actualCustomerDependencyValidationException.Should()
                .BeEquivalentTo(expectedCustomerDependencyValidationException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffset(),
                    Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectCustomerByIdAsync(someCustomer.Id),
                    Times.Never);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(expectedCustomerDependencyValidationException))),
                    Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.UpdateCustomerAsync(someCustomer),
                    Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}