using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using Smart.Api.Models.Customers;
using Smart.Api.Models.Customers.Exceptions;
using Xunit;

namespace Smart.Api.Tests.Unit.Services.Foundations.Customers
{
    public partial class CustomerServiceTests
    {
        [Fact]
        public async Task ShouldThrowValidationExceptionOnAddIfCustomerIsNullAndLogItAsync()
        {
            // given
            Customer nullCustomer = null;

            var nullCustomerException =
                new NullCustomerException();

            var expectedCustomerValidationException =
                new CustomerValidationException(nullCustomerException);

            // when
            ValueTask<Customer> addCustomerTask =
                this.customerService.AddCustomerAsync(nullCustomer);

            CustomerValidationException actualCustomerValidationException =
                await Assert.ThrowsAsync<CustomerValidationException>(
                    addCustomerTask.AsTask);

            // then
            actualCustomerValidationException.Should()
                .BeEquivalentTo(expectedCustomerValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedCustomerValidationException))),
                        Times.Once);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public async Task ShouldThrowValidationExceptionOnAddIfCustomerIsInvalidAndLogItAsync(string invalidText)
        {
            // given
            var invalidCustomer = new Customer
            {
                // Name = invalidText
            };

            var invalidCustomerException =
                new InvalidCustomerException();

            invalidCustomerException.AddData(
                key: nameof(Customer.Id),
                values: "Id is required");

            //invalidCustomerException.AddData(
            //    key: nameof(Customer.Name),
            //    values: "Text is required");

            // TODO: Add or remove data here to suit the validation needs for the Customer model

            invalidCustomerException.AddData(
                key: nameof(Customer.CreatedDate),
                values: "Date is required");

            invalidCustomerException.AddData(
                key: nameof(Customer.CreatedByUserId),
                values: "Id is required");

            invalidCustomerException.AddData(
                key: nameof(Customer.UpdatedDate),
                values: "Date is required");

            invalidCustomerException.AddData(
                key: nameof(Customer.UpdatedByUserId),
                values: "Id is required");

            var expectedCustomerValidationException =
                new CustomerValidationException(invalidCustomerException);

            // when
            ValueTask<Customer> addCustomerTask =
                this.customerService.AddCustomerAsync(invalidCustomer);

            CustomerValidationException actualCustomerValidationException =
                await Assert.ThrowsAsync<CustomerValidationException>(
                    addCustomerTask.AsTask);

            // then
            actualCustomerValidationException.Should()
                .BeEquivalentTo(expectedCustomerValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedCustomerValidationException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertCustomerAsync(It.IsAny<Customer>()),
                    Times.Never);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }
    }
}