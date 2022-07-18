using System;
using System.Linq.Expressions;
using Moq;
using Smart.Api.Brokers.DateTimes;
using Smart.Api.Brokers.Loggings;
using Smart.Api.Brokers.Storages;
using Smart.Api.Models.Customer;
using Smart.Api.Services.Foundations.Customer;
using Tynamix.ObjectFiller;
using Xeptions;

namespace Smart.Api.Tests.Unit.Services.Foundations.Customer
{
    public partial class CustomersServiceTests
    {
        private readonly Mock<IStorageBroker> storageBrokerMock;
        private readonly Mock<IDateTimeBroker> dateTimeBrokerMock;
        private readonly Mock<ILoggingBroker> loggingBrokerMock;
        private readonly ICustomersService customersService;

        public CustomersServiceTests()
        {
            this.storageBrokerMock = new Mock<IStorageBroker>();
            this.dateTimeBrokerMock = new Mock<IDateTimeBroker>();
            this.loggingBrokerMock = new Mock<ILoggingBroker>();

            this.customersService = new CustomersService(
                storageBroker: this.storageBrokerMock.Object,
                dateTimeBroker: this.dateTimeBrokerMock.Object,
                loggingBroker: this.loggingBrokerMock.Object);
        }

        private static Expression<Func<Xeption, bool>> SameExceptionAs(Xeption expectedException) =>
            actualException => actualException.SameExceptionAs(expectedException);

        private static int GetRandomNumber() =>
            new IntRange(min: 2, max: 10).GetValue();

        private static DateTimeOffset GetRandomDateTimeOffset() =>
            new DateTimeRange(earliestDate: new DateTime()).GetValue();

        private static Customers CreateRandomCustomers(DateTimeOffset dateTimeOffset) =>
            CreateCustomersFiller(dateTimeOffset).Create();

        private static Filler<Customers> CreateCustomersFiller(DateTimeOffset dateTimeOffset)
        {
            Guid userId = Guid.NewGuid();
            var filler = new Filler<Customers>();

            filler.Setup()
                .OnType<DateTimeOffset>().Use(dateTimeOffset)
                .OnProperty(customers => customers.CreatedByUserId).Use(userId)
                .OnProperty(customers => customers.UpdatedByUserId).Use(userId);

            // TODO: Complete the filler setup e.g. ignore related properties etc...

            return filler;
        }
    }
}