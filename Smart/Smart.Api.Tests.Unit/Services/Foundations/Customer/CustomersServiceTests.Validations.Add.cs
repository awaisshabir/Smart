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
        public async Task ShouldThrowValidationExceptionOnAddIfCustomersIsNullAndLogItAsync()
        {
            // given
            Customers nullCustomers = null;

            var nullCustomersException =
                new NullCustomersException();

            var expectedCustomersValidationException =
                new CustomersValidationException(nullCustomersException);

            // when
            ValueTask<Customers> addCustomersTask =
                this.customersService.AddCustomersAsync(nullCustomers);

            CustomersValidationException actualCustomersValidationException =
                await Assert.ThrowsAsync<CustomersValidationException>(
                    addCustomersTask.AsTask);

            // then
            actualCustomersValidationException.Should().BeEquivalentTo(expectedCustomersValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedCustomersValidationException))),
                        Times.Once);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }
    }
}