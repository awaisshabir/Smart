using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Smart.Api.Models.Supplier;

namespace Smart.Api.Brokers.Storages
{
    public partial class StorageBroker
    {
        public DbSet<Suppliers> Supplier { get; set; }

        public async ValueTask<Suppliers> InsertSuppliersAsync(Suppliers suppliers)
        {
            using var broker =
                new StorageBroker(this.configuration);

            EntityEntry<Suppliers> suppliersEntityEntry =
                await broker.Supplier.AddAsync(suppliers);

            await broker.SaveChangesAsync();

            return suppliersEntityEntry.Entity;
        }

        public IQueryable<Suppliers> SelectAllSupplier()
        {
            using var broker =
                new StorageBroker(this.configuration);

            return broker.Supplier;
        }
    }
}
