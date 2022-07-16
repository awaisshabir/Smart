using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Smart.Api.Models.Products;

namespace Smart.Api.Brokers.Storages
{
    public partial class StorageBroker
    {
        public DbSet<Product> Products { get; set; }

        public async ValueTask<Product> InsertProductAsync(Product product)
        {
            using var broker =
                new StorageBroker(this.configuration);

            EntityEntry<Product> productEntityEntry =
                await broker.Products.AddAsync(product);

            await broker.SaveChangesAsync();

            return productEntityEntry.Entity;
        }
    }
}
