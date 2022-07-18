using System;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using Smart.Api.Models.Customer;
using Smart.Api.Models.Customer.Exceptions;
using Xunit;

namespace Smart.Api.Tests.Unit.Services.Foundations.Customer
{
    public partial class CustomersServiceTests
    {
        [Fact]
        public async Task ShouldThrowValidationExceptionOnRetrieveByIdIfIdIsInvalidAndLogItAsync()
        {
            // given
            var invalidCustomersId = Guid.Empty;

            var invalidCustomersException =
                new InvalidCustomersException();

            invalidCustomersException.AddData(
                key: nameof(Customers.Id),
                values: "Id is required");

            var expectedCustomersValidationException =
                new CustomersValidationException(invalidCustomersException);

            // when
            ValueTask<Customers> retrieveCustomersByIdTask =
                this.customersService.RetrieveCustomersByIdAsync(invalidCustomersId);

            CustomersValidationException actualCustomersValidationException =
                await Assert.ThrowsAsync<CustomersValidationException>(
                    retrieveCustomersByIdTask.AsTask);

            // then
            actualCustomersValidationException.Should()
                .BeEquivalentTo(expectedCustomersValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedCustomersValidationException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectCustomersByIdAsync(It.IsAny<Guid>()),
                    Times.Never);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowNotFoundExceptionOnRetrieveByIdIfCustomersIsNotFoundAndLogItAsync()
        {
            //given
            Guid someCustomersId = Guid.NewGuid();
            Customers noCustomers = null;

            var notFoundCustomersException =
                new NotFoundCustomersException(someCustomersId);

            var expectedCustomersValidationException =
                new CustomersValidationException(notFoundCustomersException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectCustomersByIdAsync(It.IsAny<Guid>()))
                    .ReturnsAsync(noCustomers);

            //when
            ValueTask<Customers> retrieveCustomersByIdTask =
                this.customersService.RetrieveCustomersByIdAsync(someCustomersId);

            CustomersValidationException actualCustomersValidationException =
                await Assert.ThrowsAsync<CustomersValidationException>(
                    retrieveCustomersByIdTask.AsTask);

            //then
            actualCustomersValidationException.Should().BeEquivalentTo(expectedCustomersValidationException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectCustomersByIdAsync(It.IsAny<Guid>()),
                    Times.Once());

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedCustomersValidationException))),
                        Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }
    }
}