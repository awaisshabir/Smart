using System;
using System.Threading.Tasks;
using FluentAssertions;
using Force.DeepCloner;
using Moq;
using Smart.Api.Models.Customer;
using Xunit;

namespace Smart.Api.Tests.Unit.Services.Foundations.Customer
{
    public partial class CustomersServiceTests
    {
        [Fact]
        public async Task ShouldRemoveCustomersByIdAsync()
        {
            // given
            Guid randomId = Guid.NewGuid();
            Guid inputCustomersId = randomId;
            Customers randomCustomers = CreateRandomCustomers();
            Customers storageCustomers = randomCustomers;
            Customers expectedInputCustomers = storageCustomers;
            Customers deletedCustomers = expectedInputCustomers;
            Customers expectedCustomers = deletedCustomers.DeepClone();

            this.storageBrokerMock.Setup(broker =>
                broker.SelectCustomersByIdAsync(inputCustomersId))
                    .ReturnsAsync(storageCustomers);

            this.storageBrokerMock.Setup(broker =>
                broker.DeleteCustomersAsync(expectedInputCustomers))
                    .ReturnsAsync(deletedCustomers);

            // when
            Customers actualCustomers = await this.customersService
                .RemoveCustomersByIdAsync(inputCustomersId);

            // then
            actualCustomers.Should().BeEquivalentTo(expectedCustomers);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectCustomersByIdAsync(inputCustomersId),
                    Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.DeleteCustomersAsync(expectedInputCustomers),
                    Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }
    }
}