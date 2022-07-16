using System.Threading.Tasks;
using Smart.Api.Models.Customers;

namespace Smart.Api.Services.Foundations.Customers
{
    public interface ICustomerService
    {
        ValueTask<Customer> AddCustomerAsync(Customer customer);
    }
}