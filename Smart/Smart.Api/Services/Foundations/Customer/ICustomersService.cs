using System.Linq;
using System.Threading.Tasks;
using Smart.Api.Models.Customer;

namespace Smart.Api.Services.Foundations.Customer
{
    public interface ICustomersService
    {
        ValueTask<Customers> AddCustomersAsync(Customers customers);
        IQueryable<Customers> RetrieveAllCustomer();
    }
}