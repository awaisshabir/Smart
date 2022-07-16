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
            actualCustomerValidationException.Should().BeEquivalentTo(expectedCustomerValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedCustomerValidationException))),
                        Times.Once);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }
    }
}