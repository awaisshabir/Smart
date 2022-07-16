using System;
using Moq;
using Smart.Api.Brokers.DateTimes;
using Smart.Api.Brokers.Loggings;
using Smart.Api.Brokers.Storages;
using Smart.Api.Models.Products;
using Smart.Api.Services.Foundations.Products;
using Tynamix.ObjectFiller;

namespace Smart.Api.Tests.Unit.Services.Foundations.Products
{
    public partial class ProductServiceTests
    {
        private readonly Mock<IStorageBroker> storageBrokerMock;
        private readonly Mock<IDateTimeBroker> dateTimeBrokerMock;
        private readonly Mock<ILoggingBroker> loggingBrokerMock;
        private readonly IProductService productService;

        public ProductServiceTests()
        {
            this.storageBrokerMock = new Mock<IStorageBroker>();
            this.dateTimeBrokerMock = new Mock<IDateTimeBroker>();
            this.loggingBrokerMock = new Mock<ILoggingBroker>();

            this.productService = new ProductService(
                storageBroker: this.storageBrokerMock.Object,
                dateTimeBroker: this.dateTimeBrokerMock.Object,
                loggingBroker: this.loggingBrokerMock.Object);
        }

        private static DateTimeOffset GetRandomDateTimeOffset() =>
            new DateTimeRange(earliestDate: new DateTime()).GetValue();

        private static Product CreateRandomProduct(DateTimeOffset dateTimeOffset) =>
            CreateProductFiller(dateTimeOffset).Create();

        private static Filler<Product> CreateProductFiller(DateTimeOffset dateTimeOffset)
        {
            Guid userId = Guid.NewGuid();
            var filler = new Filler<Product>();

            filler.Setup()
                .OnType<DateTimeOffset>().Use(dateTimeOffset)
                .OnProperty(product => product.CreatedByUserId).Use(userId)
                .OnProperty(product => product.UpdatedByUserId).Use(userId);

            // TODO: Complete the filler setup e.g. ignore related properties etc...

            return filler;
        }
    }
}