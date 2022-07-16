using System;
using System.Threading.Tasks;
using FluentAssertions;
using Force.DeepCloner;
using Moq;
using Smart.Api.Models.Customers;
using Xunit;

namespace Smart.Api.Tests.Unit.Services.Foundations.Customers
{
    public partial class CustomerServiceTests
    {
        [Fact]
        public async Task ShouldModifyCustomerAsync()
        {
            // given
            DateTimeOffset randomDateTimeOffset = GetRandomDateTimeOffset();
            Customer randomCustomer = CreateRandomModifyCustomer(randomDateTimeOffset);
            Customer inputCustomer = randomCustomer;
            Customer storageCustomer = inputCustomer.DeepClone();
            storageCustomer.UpdatedDate = randomCustomer.CreatedDate;
            Customer updatedCustomer = inputCustomer;
            Customer expectedCustomer = updatedCustomer.DeepClone();
            Guid customerId = inputCustomer.Id;

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffset())
                    .Returns(randomDateTimeOffset);

            this.storageBrokerMock.Setup(broker =>
                broker.UpdateCustomerAsync(inputCustomer))
                    .ReturnsAsync(updatedCustomer);

            // when
            Customer actualCustomer =
                await this.customerService.ModifyCustomerAsync(inputCustomer);

            // then
            actualCustomer.Should().BeEquivalentTo(expectedCustomer);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffset(),
                    Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.UpdateCustomerAsync(inputCustomer),
                    Times.Once);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}