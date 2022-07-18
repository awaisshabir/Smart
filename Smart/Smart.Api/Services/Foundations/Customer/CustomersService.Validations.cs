using Smart.Api.Models.Customer;
using Smart.Api.Models.Customer.Exceptions;

namespace Smart.Api.Services.Foundations.Customer
{
    public partial class CustomersService
    {
        private void ValidateCustomersOnAdd(Customers customers)
        {
            ValidateCustomersIsNotNull(customers);
        }

        private static void ValidateCustomersIsNotNull(Customers customers)
        {
            if (customers is null)
            {
                throw new NullCustomersException();
            }
        }
    }
}