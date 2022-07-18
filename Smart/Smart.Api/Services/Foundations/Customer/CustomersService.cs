using System;
using System.Linq;
using System.Threading.Tasks;
using Smart.Api.Brokers.DateTimes;
using Smart.Api.Brokers.Loggings;
using Smart.Api.Brokers.Storages;
using Smart.Api.Models.Customer;

namespace Smart.Api.Services.Foundations.Customer
{
    public partial class CustomersService : ICustomersService
    {
        private readonly IStorageBroker storageBroker;
        private readonly IDateTimeBroker dateTimeBroker;
        private readonly ILoggingBroker loggingBroker;

        public CustomersService(
            IStorageBroker storageBroker,
            IDateTimeBroker dateTimeBroker,
            ILoggingBroker loggingBroker)
        {
            this.storageBroker = storageBroker;
            this.dateTimeBroker = dateTimeBroker;
            this.loggingBroker = loggingBroker;
        }

        public ValueTask<Customers> AddCustomersAsync(Customers customers) =>
            TryCatch(async () =>
            {
                ValidateCustomersOnAdd(customers);

                return await this.storageBroker.InsertCustomersAsync(customers);
            });

        public IQueryable<Customers> RetrieveAllCustomer() =>
            TryCatch(() => this.storageBroker.SelectAllCustomer());

        public ValueTask<Customers> RetrieveCustomersByIdAsync(Guid customersId) =>
            throw new NotImplementedException();
    }
}