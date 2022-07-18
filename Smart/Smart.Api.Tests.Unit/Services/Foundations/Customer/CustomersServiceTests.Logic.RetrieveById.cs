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
        public async Task ShouldRetrieveCustomersByIdAsync()
        {
            // given
            Customers randomCustomers = CreateRandomCustomers();
            Customers inputCustomers = randomCustomers;
            Customers storageCustomers = randomCustomers;
            Customers expectedCustomers = storageCustomers.DeepClone();

            this.storageBrokerMock.Setup(broker =>
                broker.SelectCustomersByIdAsync(inputCustomers.Id))
                    .ReturnsAsync(storageCustomers);

            // when
            Customers actualCustomers =
                await this.customersService.RetrieveCustomersByIdAsync(inputCustomers.Id);

            // then
            actualCustomers.Should().BeEquivalentTo(expectedCustomers);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectCustomersByIdAsync(inputCustomers.Id),
                    Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}