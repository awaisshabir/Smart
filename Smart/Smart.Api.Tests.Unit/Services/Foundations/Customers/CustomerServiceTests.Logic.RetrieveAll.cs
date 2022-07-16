using System.Linq;
using FluentAssertions;
using Moq;
using Smart.Api.Models.Customers;
using Xunit;

namespace Smart.Api.Tests.Unit.Services.Foundations.Customers
{
    public partial class CustomerServiceTests
    {
        [Fact]
        public void ShouldReturnCustomers()
        {
            // given
            IQueryable<Customer> randomCustomers = CreateRandomCustomers();
            IQueryable<Customer> storageCustomers = randomCustomers;
            IQueryable<Customer> expectedCustomers = storageCustomers;

            this.storageBrokerMock.Setup(broker =>
                broker.SelectAllCustomers())
                    .Returns(storageCustomers);

            // when
            IQueryable<Customer> actualCustomers =
                this.customerService.RetrieveAllCustomers();

            // then
            actualCustomers.Should().BeEquivalentTo(expectedCustomers);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectAllCustomers(),
                    Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}