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
        public async Task ShouldRetrieveCustomerByIdAsync()
        {
            // given
            Customer randomCustomer = CreateRandomCustomer();
            Customer inputCustomer = randomCustomer;
            Customer storageCustomer = randomCustomer;
            Customer expectedCustomer = storageCustomer.DeepClone();

            this.storageBrokerMock.Setup(broker =>
                broker.SelectCustomerByIdAsync(inputCustomer.Id))
                    .ReturnsAsync(storageCustomer);

            // when
            Customer actualCustomer =
                await this.customerService.RetrieveCustomerByIdAsync(inputCustomer.Id);

            // then
            actualCustomer.Should().BeEquivalentTo(expectedCustomer);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectCustomerByIdAsync(inputCustomer.Id),
                    Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}