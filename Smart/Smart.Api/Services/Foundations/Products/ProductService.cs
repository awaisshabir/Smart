using System;
using System.Linq;
using System.Threading.Tasks;
using Smart.Api.Brokers.DateTimes;
using Smart.Api.Brokers.Loggings;
using Smart.Api.Brokers.Storages;
using Smart.Api.Models.Products;

namespace Smart.Api.Services.Foundations.Products
{
    public partial class ProductService : IProductService
    {
        private readonly IStorageBroker storageBroker;
        private readonly IDateTimeBroker dateTimeBroker;
        private readonly ILoggingBroker loggingBroker;

        public ProductService(
            IStorageBroker storageBroker,
            IDateTimeBroker dateTimeBroker,
            ILoggingBroker loggingBroker)
        {
            this.storageBroker = storageBroker;
            this.dateTimeBroker = dateTimeBroker;
            this.loggingBroker = loggingBroker;
        }

        public ValueTask<Product> AddProductAsync(Product product) =>
            TryCatch(async () =>
            {
                ValidateProductOnAdd(product);

                return await this.storageBroker.InsertProductAsync(product);
            });

        public IQueryable<Product> RetrieveAllProducts() =>
            TryCatch(() => this.storageBroker.SelectAllProducts());

        public ValueTask<Product> RetrieveProductByIdAsync(Guid productId) =>
            TryCatch(async () =>
            {
                ValidateProductId(productId);

                Product maybeProduct = await this.storageBroker
                    .SelectProductByIdAsync(productId);

                ValidateStorageProduct(maybeProduct, productId);

                return maybeProduct;
            });

        public ValueTask<Product> ModifyProductAsync(Product product) =>
            throw new NotImplementedException();
    }
}