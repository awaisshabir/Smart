using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Smart.Api.Models.Customer;

namespace Smart.Api.Brokers.Storages
{
    public partial class StorageBroker
    {
        public DbSet<Customers> Customer { get; set; }

        public async ValueTask<Customers> InsertCustomersAsync(Customers customers)
        {
            using var broker =
                new StorageBroker(this.configuration);

            EntityEntry<Customers> customersEntityEntry =
                await broker.Customer.AddAsync(customers);

            await broker.SaveChangesAsync();

            return customersEntityEntry.Entity;
        }

        public IQueryable<Customers> SelectAllCustomer()
        {
            using var broker =
                new StorageBroker(this.configuration);

            return broker.Customer;
        }

        public async ValueTask<Customers> SelectCustomersByIdAsync(Guid customersId)
        {
            using var broker =
                new StorageBroker(this.configuration);

            return await broker.Customer.FindAsync(customersId);
        }

        public async ValueTask<Customers> UpdateCustomersAsync(Customers customers)
        {
            using var broker =
                new StorageBroker(this.configuration);

            EntityEntry<Customers> customersEntityEntry =
                broker.Customer.Update(customers);

            await broker.SaveChangesAsync();

            return customersEntityEntry.Entity;
        }
    }
}
