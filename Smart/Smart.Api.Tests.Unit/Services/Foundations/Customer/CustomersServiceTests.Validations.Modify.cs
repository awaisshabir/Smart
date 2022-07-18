using System;
using System.Threading.Tasks;
using FluentAssertions;
using Force.DeepCloner;
using Moq;
using Smart.Api.Models.Customer;
using Smart.Api.Models.Customer.Exceptions;
using Xunit;

namespace Smart.Api.Tests.Unit.Services.Foundations.Customer
{
    public partial class CustomersServiceTests
    {
        [Fact]
        public async Task ShouldThrowValidationExceptionOnModifyIfCustomersIsNullAndLogItAsync()
        {
            // given
            Customers nullCustomers = null;
            var nullCustomersException = new NullCustomersException();

            var expectedCustomersValidationException =
                new CustomersValidationException(nullCustomersException);

            // when
            ValueTask<Customers> modifyCustomersTask =
                this.customersService.ModifyCustomersAsync(nullCustomers);

            CustomersValidationException actualCustomersValidationException =
                await Assert.ThrowsAsync<CustomersValidationException>(
                    modifyCustomersTask.AsTask);

            // then
            actualCustomersValidationException.Should()
                .BeEquivalentTo(expectedCustomersValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedCustomersValidationException))),
                        Times.Once);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffset(),
                    Times.Never);

            this.storageBrokerMock.Verify(broker =>
                broker.UpdateCustomersAsync(It.IsAny<Customers>()),
                    Times.Never);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public async Task ShouldThrowValidationExceptionOnModifyIfCustomersIsInvalidAndLogItAsync(string invalidText)
        {
            // given 
            var invalidCustomers = new Customers
            {
                //Name = invalidText,
            };

            var invalidCustomersException = new InvalidCustomersException();

            invalidCustomersException.AddData(
                key: nameof(Customers.Id),
                values: "Id is required");

            //invalidCustomersException.AddData(
            //    key: nameof(Customers.Name),
            //    values: "Text is required");

            // TODO: Add or remove data here to suit the validation needs for the Customers model

            invalidCustomersException.AddData(
                key: nameof(Customers.CreatedDate),
                values: "Date is required");

            invalidCustomersException.AddData(
                key: nameof(Customers.CreatedByUserId),
                values: "Id is required");

            invalidCustomersException.AddData(
                key: nameof(Customers.UpdatedDate),
                values:
                new[] {
                    "Date is required",
                    $"Date is the same as {nameof(Customers.CreatedDate)}"
                });

            invalidCustomersException.AddData(
                key: nameof(Customers.UpdatedByUserId),
                values: "Id is required");

            var expectedCustomersValidationException =
                new CustomersValidationException(invalidCustomersException);

            // when
            ValueTask<Customers> modifyCustomersTask =
                this.customersService.ModifyCustomersAsync(invalidCustomers);

            CustomersValidationException actualCustomersValidationException =
                await Assert.ThrowsAsync<CustomersValidationException>(
                    modifyCustomersTask.AsTask);

            //then
            actualCustomersValidationException.Should()
                .BeEquivalentTo(expectedCustomersValidationException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffset(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedCustomersValidationException))),
                        Times.Once());

            this.storageBrokerMock.Verify(broker =>
                broker.UpdateCustomersAsync(It.IsAny<Customers>()),
                    Times.Never);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowValidationExceptionOnModifyIfUpdatedDateIsSameAsCreatedDateAndLogItAsync()
        {
            // given
            DateTimeOffset randomDateTimeOffset = GetRandomDateTimeOffset();
            Customers randomCustomers = CreateRandomCustomers(randomDateTimeOffset);
            Customers invalidCustomers = randomCustomers;
            var invalidCustomersException = new InvalidCustomersException();

            invalidCustomersException.AddData(
                key: nameof(Customers.UpdatedDate),
                values: $"Date is the same as {nameof(Customers.CreatedDate)}");

            var expectedCustomersValidationException =
                new CustomersValidationException(invalidCustomersException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffset())
                    .Returns(randomDateTimeOffset);

            // when
            ValueTask<Customers> modifyCustomersTask =
                this.customersService.ModifyCustomersAsync(invalidCustomers);

            CustomersValidationException actualCustomersValidationException =
                await Assert.ThrowsAsync<CustomersValidationException>(
                    modifyCustomersTask.AsTask);

            // then
            actualCustomersValidationException.Should()
                .BeEquivalentTo(expectedCustomersValidationException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffset(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedCustomersValidationException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectCustomersByIdAsync(invalidCustomers.Id),
                    Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Theory]
        [MemberData(nameof(MinutesBeforeOrAfter))]
        public async Task ShouldThrowValidationExceptionOnModifyIfUpdatedDateIsNotRecentAndLogItAsync(int minutes)
        {
            // given
            DateTimeOffset randomDateTimeOffset = GetRandomDateTimeOffset();
            Customers randomCustomers = CreateRandomCustomers(randomDateTimeOffset);
            randomCustomers.UpdatedDate = randomDateTimeOffset.AddMinutes(minutes);

            var invalidCustomersException =
                new InvalidCustomersException();

            invalidCustomersException.AddData(
                key: nameof(Customers.UpdatedDate),
                values: "Date is not recent");

            var expectedCustomersValidatonException =
                new CustomersValidationException(invalidCustomersException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffset())
                .Returns(randomDateTimeOffset);

            // when
            ValueTask<Customers> modifyCustomersTask =
                this.customersService.ModifyCustomersAsync(randomCustomers);

            CustomersValidationException actualCustomersValidationException =
                await Assert.ThrowsAsync<CustomersValidationException>(
                    modifyCustomersTask.AsTask);

            // then
            actualCustomersValidationException.Should()
                .BeEquivalentTo(expectedCustomersValidatonException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffset(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedCustomersValidatonException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectCustomersByIdAsync(It.IsAny<Guid>()),
                    Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowValidationExceptionOnModifyIfCustomersDoesNotExistAndLogItAsync()
        {
            // given
            DateTimeOffset randomDateTimeOffset = GetRandomDateTimeOffset();
            Customers randomCustomers = CreateRandomModifyCustomers(randomDateTimeOffset);
            Customers nonExistCustomers = randomCustomers;
            Customers nullCustomers = null;

            var notFoundCustomersException =
                new NotFoundCustomersException(nonExistCustomers.Id);

            var expectedCustomersValidationException =
                new CustomersValidationException(notFoundCustomersException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectCustomersByIdAsync(nonExistCustomers.Id))
                .ReturnsAsync(nullCustomers);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffset())
                .Returns(randomDateTimeOffset);

            // when 
            ValueTask<Customers> modifyCustomersTask =
                this.customersService.ModifyCustomersAsync(nonExistCustomers);

            CustomersValidationException actualCustomersValidationException =
                await Assert.ThrowsAsync<CustomersValidationException>(
                    modifyCustomersTask.AsTask);

            // then
            actualCustomersValidationException.Should()
                .BeEquivalentTo(expectedCustomersValidationException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectCustomersByIdAsync(nonExistCustomers.Id),
                    Times.Once);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffset(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedCustomersValidationException))),
                        Times.Once);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowValidationExceptionOnModifyIfStorageCreatedDateNotSameAsCreatedDateAndLogItAsync()
        {
            // given
            int randomNumber = GetRandomNegativeNumber();
            int randomMinutes = randomNumber;
            DateTimeOffset randomDateTimeOffset = GetRandomDateTimeOffset();
            Customers randomCustomers = CreateRandomModifyCustomers(randomDateTimeOffset);
            Customers invalidCustomers = randomCustomers.DeepClone();
            Customers storageCustomers = invalidCustomers.DeepClone();
            storageCustomers.CreatedDate = storageCustomers.CreatedDate.AddMinutes(randomMinutes);
            storageCustomers.UpdatedDate = storageCustomers.UpdatedDate.AddMinutes(randomMinutes);
            var invalidCustomersException = new InvalidCustomersException();

            invalidCustomersException.AddData(
                key: nameof(Customers.CreatedDate),
                values: $"Date is not the same as {nameof(Customers.CreatedDate)}");

            var expectedCustomersValidationException =
                new CustomersValidationException(invalidCustomersException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectCustomersByIdAsync(invalidCustomers.Id))
                .ReturnsAsync(storageCustomers);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffset())
                .Returns(randomDateTimeOffset);

            // when
            ValueTask<Customers> modifyCustomersTask =
                this.customersService.ModifyCustomersAsync(invalidCustomers);

            CustomersValidationException actualCustomersValidationException =
                await Assert.ThrowsAsync<CustomersValidationException>(
                    modifyCustomersTask.AsTask);

            // then
            actualCustomersValidationException.Should()
                .BeEquivalentTo(expectedCustomersValidationException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectCustomersByIdAsync(invalidCustomers.Id),
                    Times.Once);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffset(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
               broker.LogError(It.Is(SameExceptionAs(
                   expectedCustomersValidationException))),
                       Times.Once);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowValidationExceptionOnModifyIfCreatedUserIdDontMacthStorageAndLogItAsync()
        {
            // given
            DateTimeOffset randomDateTimeOffset = GetRandomDateTimeOffset();
            Customers randomCustomers = CreateRandomModifyCustomers(randomDateTimeOffset);
            Customers invalidCustomers = randomCustomers.DeepClone();
            Customers storageCustomers = invalidCustomers.DeepClone();
            invalidCustomers.CreatedByUserId = Guid.NewGuid();
            storageCustomers.UpdatedDate = storageCustomers.CreatedDate;

            var invalidCustomersException = new InvalidCustomersException();

            invalidCustomersException.AddData(
                key: nameof(Customers.CreatedByUserId),
                values: $"Id is not the same as {nameof(Customers.CreatedByUserId)}");

            var expectedCustomersValidationException =
                new CustomersValidationException(invalidCustomersException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectCustomersByIdAsync(invalidCustomers.Id))
                .ReturnsAsync(storageCustomers);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffset())
                .Returns(randomDateTimeOffset);

            // when
            ValueTask<Customers> modifyCustomersTask =
                this.customersService.ModifyCustomersAsync(invalidCustomers);

            CustomersValidationException actualCustomersValidationException =
                await Assert.ThrowsAsync<CustomersValidationException>(
                    modifyCustomersTask.AsTask);

            // then
            actualCustomersValidationException.Should().BeEquivalentTo(expectedCustomersValidationException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectCustomersByIdAsync(invalidCustomers.Id),
                    Times.Once);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffset(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
               broker.LogError(It.Is(SameExceptionAs(
                   expectedCustomersValidationException))),
                       Times.Once);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}