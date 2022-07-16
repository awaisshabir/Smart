using System;
using System.Linq;
using System.Threading.Tasks;
using Smart.Api.Models.Products;

namespace Smart.Api.Brokers.Storages
{
    public partial interface IStorageBroker
    {
        ValueTask<Product> InsertProductAsync(Product product);
        IQueryable<Product> SelectAllProducts();
    }
}
