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
        public async Task ShouldThrowValidationExceptionOnModifyIfCustomersIsNullAndLogItAsync()
        {
            // given
            Customers nullCustomers = null;
            var nullCustomersException = new NullCustomersException();

            var expectedCustomersValidationException =
                new CustomersValidationException(nullCustomersException);

            // when
            ValueTask<Customers> modifyCustomersTask =
                this.customersService.ModifyCustomersAsync(nullCustomers);

            CustomersValidationException actualCustomersValidationException =
                await Assert.ThrowsAsync<CustomersValidationException>(
                    modifyCustomersTask.AsTask);

            // then
            actualCustomersValidationException.Should()
                .BeEquivalentTo(expectedCustomersValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedCustomersValidationException))),
                        Times.Once);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffset(),
                    Times.Never);

            this.storageBrokerMock.Verify(broker =>
                broker.UpdateCustomersAsync(It.IsAny<Customers>()),
                    Times.Never);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public async Task ShouldThrowValidationExceptionOnModifyIfCustomersIsInvalidAndLogItAsync(string invalidText)
        {
            // given 
            var invalidCustomers = new Customers
            {
                //Name = invalidText,
            };

            var invalidCustomersException = new InvalidCustomersException();

            invalidCustomersException.AddData(
                key: nameof(Customers.Id),
                values: "Id is required");

            //invalidCustomersException.AddData(
            //    key: nameof(Customers.Name),
            //    values: "Text is required");

            // TODO: Add or remove data here to suit the validation needs for the Customers model

            invalidCustomersException.AddData(
                key: nameof(Customers.CreatedDate),
                values: "Date is required");

            invalidCustomersException.AddData(
                key: nameof(Customers.CreatedByUserId),
                values: "Id is required");

            invalidCustomersException.AddData(
                key: nameof(Customers.UpdatedDate),
                values: "Date is required");

            invalidCustomersException.AddData(
                key: nameof(Customers.UpdatedByUserId),
                values: "Id is required");

            var expectedCustomersValidationException =
                new CustomersValidationException(invalidCustomersException);

            // when
            ValueTask<Customers> modifyCustomersTask =
                this.customersService.ModifyCustomersAsync(invalidCustomers);

            CustomersValidationException actualCustomersValidationException =
                await Assert.ThrowsAsync<CustomersValidationException>(
                    modifyCustomersTask.AsTask);

            //then
            actualCustomersValidationException.Should().BeEquivalentTo(expectedCustomersValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedCustomersValidationException))),
                        Times.Once());

            this.storageBrokerMock.Verify(broker =>
                broker.UpdateCustomersAsync(It.IsAny<Customers>()),
                    Times.Never);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }
    }
}