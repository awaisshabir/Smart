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
        public async Task ShouldModifyCustomersAsync()
        {
            // given
            DateTimeOffset randomDateTimeOffset = GetRandomDateTimeOffset();
            Customers randomCustomers = CreateRandomModifyCustomers(randomDateTimeOffset);
            Customers inputCustomers = randomCustomers;
            Customers storageCustomers = inputCustomers.DeepClone();
            storageCustomers.UpdatedDate = randomCustomers.CreatedDate;
            Customers updatedCustomers = inputCustomers;
            Customers expectedCustomers = updatedCustomers.DeepClone();
            Guid customersId = inputCustomers.Id;

            this.storageBrokerMock.Setup(broker =>
                broker.UpdateCustomersAsync(inputCustomers))
                    .ReturnsAsync(updatedCustomers);

            // when
            Customers actualCustomers =
                await this.customersService.ModifyCustomersAsync(inputCustomers);

            // then
            actualCustomers.Should().BeEquivalentTo(expectedCustomers);

            this.storageBrokerMock.Verify(broker =>
                broker.UpdateCustomersAsync(inputCustomers),
                    Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }
    }
}