using Smart.Api.Models.Customers;
using Smart.Api.Models.Customers.Exceptions;

namespace Smart.Api.Services.Foundations.Customers
{
    public partial class CustomerService
    {
        private void ValidateCustomerOnAdd(Customer customer)
        {
            ValidateCustomerIsNotNull(customer);
        }

        private static void ValidateCustomerIsNotNull(Customer customer)
        {
            if (customer is null)
            {
                throw new NullCustomerException();
            }
        }
    }
}