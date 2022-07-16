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
        public async Task ShouldAddCustomerAsync()
        {
            // given
            DateTimeOffset randomDateTimeOffset =
                GetRandomDateTimeOffset();

            Customer randomCustomer = CreateRandomCustomer(randomDateTimeOffset);
            Customer inputCustomer = randomCustomer;
            Customer storageCustomer = inputCustomer;
            Customer expectedCustomer = storageCustomer.DeepClone();

            this.storageBrokerMock.Setup(broker =>
                broker.InsertCustomerAsync(inputCustomer))
                    .ReturnsAsync(storageCustomer);

            // when
            Customer actualCustomer = await this.customerService
                .AddCustomerAsync(inputCustomer);

            // then
            actualCustomer.Should().BeEquivalentTo(expectedCustomer);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertCustomerAsync(inputCustomer),
                    Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }
    }
}