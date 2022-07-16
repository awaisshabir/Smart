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
        public async Task ShouldRemoveCustomerByIdAsync()
        {
            // given
            Guid randomId = Guid.NewGuid();
            Guid inputCustomerId = randomId;
            Customer randomCustomer = CreateRandomCustomer();
            Customer storageCustomer = randomCustomer;
            Customer expectedInputCustomer = storageCustomer;
            Customer deletedCustomer = expectedInputCustomer;
            Customer expectedCustomer = deletedCustomer.DeepClone();

            this.storageBrokerMock.Setup(broker =>
                broker.SelectCustomerByIdAsync(inputCustomerId))
                    .ReturnsAsync(storageCustomer);

            this.storageBrokerMock.Setup(broker =>
                broker.DeleteCustomerAsync(expectedInputCustomer))
                    .ReturnsAsync(deletedCustomer);

            // when
            Customer actualCustomer = await this.customerService
                .RemoveCustomerByIdAsync(inputCustomerId);

            // then
            actualCustomer.Should().BeEquivalentTo(expectedCustomer);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectCustomerByIdAsync(inputCustomerId),
                    Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.DeleteCustomerAsync(expectedInputCustomer),
                    Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }
    }
}