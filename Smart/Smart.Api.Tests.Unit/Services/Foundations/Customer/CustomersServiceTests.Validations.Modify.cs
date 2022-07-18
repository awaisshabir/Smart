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
    }
}