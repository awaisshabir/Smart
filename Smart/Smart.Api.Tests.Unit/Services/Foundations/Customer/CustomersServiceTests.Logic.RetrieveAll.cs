using System.Linq;
using FluentAssertions;
using Moq;
using Smart.Api.Models.Customer;
using Xunit;

namespace Smart.Api.Tests.Unit.Services.Foundations.Customer
{
    public partial class CustomersServiceTests
    {
        [Fact]
        public void ShouldReturnCustomer()
        {
            // given
            IQueryable<Customers> randomCustomer = CreateRandomCustomer();
            IQueryable<Customers> storageCustomer = randomCustomer;
            IQueryable<Customers> expectedCustomer = storageCustomer;

            this.storageBrokerMock.Setup(broker =>
                broker.SelectAllCustomer())
                    .Returns(storageCustomer);

            // when
            IQueryable<Customers> actualCustomer =
                this.customersService.RetrieveAllCustomer();

            // then
            actualCustomer.Should().BeEquivalentTo(expectedCustomer);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectAllCustomer(),
                    Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}