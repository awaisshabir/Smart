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
        public async Task ShouldAddCustomersAsync()
        {
            // given
            DateTimeOffset randomDateTimeOffset =
                GetRandomDateTimeOffset();

            Customers randomCustomers = CreateRandomCustomers(randomDateTimeOffset);
            Customers inputCustomers = randomCustomers;
            Customers storageCustomers = inputCustomers;
            Customers expectedCustomers = storageCustomers.DeepClone();

            this.storageBrokerMock.Setup(broker =>
                broker.InsertCustomersAsync(inputCustomers))
                    .ReturnsAsync(storageCustomers);

            // when
            Customers actualCustomers = await this.customersService
                .AddCustomersAsync(inputCustomers);

            // then
            actualCustomers.Should().BeEquivalentTo(expectedCustomers);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertCustomersAsync(inputCustomers),
                    Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }
    }
}