using System;
using System.Linq;
using System.Threading.Tasks;
using Smart.Api.Models.Customer;

namespace Smart.Api.Brokers.Storages
{
    public partial interface IStorageBroker
    {
        ValueTask<Customers> InsertCustomersAsync(Customers customers);
        IQueryable<Customers> SelectAllCustomer();
        ValueTask<Customers> SelectCustomersByIdAsync(Guid customersId);
    }
}
