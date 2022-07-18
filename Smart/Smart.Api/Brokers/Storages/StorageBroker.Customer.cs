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
    }
}
