using System;
using System.Linq;
using System.Threading.Tasks;
using Smart.Api.Brokers.DateTimes;
using Smart.Api.Brokers.Loggings;
using Smart.Api.Brokers.Storages;
using Smart.Api.Models.Customers;

namespace Smart.Api.Services.Foundations.Customers
{
    public partial class CustomerService : ICustomerService
    {
        private readonly IStorageBroker storageBroker;
        private readonly IDateTimeBroker dateTimeBroker;
        private readonly ILoggingBroker loggingBroker;

        public CustomerService(
            IStorageBroker storageBroker,
            IDateTimeBroker dateTimeBroker,
            ILoggingBroker loggingBroker)
        {
            this.storageBroker = storageBroker;
            this.dateTimeBroker = dateTimeBroker;
            this.loggingBroker = loggingBroker;
        }

        public ValueTask<Customer> AddCustomerAsync(Customer customer) =>
            TryCatch(async () =>
            {
                ValidateCustomerOnAdd(customer);

                return await this.storageBroker.InsertCustomerAsync(customer);
            });

        public IQueryable<Customer> RetrieveAllCustomers() =>
            TryCatch(() => this.storageBroker.SelectAllCustomers());

        public ValueTask<Customer> RetrieveCustomerByIdAsync(Guid customerId) =>
            TryCatch(async () =>
            {
                ValidateCustomerId(customerId);

                Customer maybeCustomer = await this.storageBroker
                    .SelectCustomerByIdAsync(customerId);

                ValidateStorageCustomer(maybeCustomer, customerId);

                return maybeCustomer;
            });

        public ValueTask<Customer> ModifyCustomerAsync(Customer customer) =>
            TryCatch(async () =>
            {
                ValidateCustomerOnModify(customer);

                return await this.storageBroker.UpdateCustomerAsync(customer);
            });
    }
}