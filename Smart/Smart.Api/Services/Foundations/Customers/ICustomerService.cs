using System;
using System.Linq;
using System.Threading.Tasks;
using Smart.Api.Models.Customers;

namespace Smart.Api.Services.Foundations.Customers
{
    public interface ICustomerService
    {
        ValueTask<Customer> AddCustomerAsync(Customer customer);
        IQueryable<Customer> RetrieveAllCustomers();
        ValueTask<Customer> RetrieveCustomerByIdAsync(Guid customerId);
        ValueTask<Customer> ModifyCustomerAsync(Customer customer);
    }
}