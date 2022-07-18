using System;
using System.Linq;
using System.Threading.Tasks;
using Smart.Api.Models.Supplier;

namespace Smart.Api.Brokers.Storages
{
    public partial interface IStorageBroker
    {
        ValueTask<Suppliers> InsertSuppliersAsync(Suppliers suppliers);
        IQueryable<Suppliers> SelectAllSupplier();
        ValueTask<Suppliers> SelectSuppliersByIdAsync(Guid suppliersId);
        ValueTask<Suppliers> UpdateSuppliersAsync(Suppliers suppliers);
    }
}
