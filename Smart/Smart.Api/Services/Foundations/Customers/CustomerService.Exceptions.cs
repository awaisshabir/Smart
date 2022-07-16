using System.Threading.Tasks;
using Smart.Api.Models.Customers;
using Smart.Api.Models.Customers.Exceptions;
using Xeptions;

namespace Smart.Api.Services.Foundations.Customers
{
    public partial class CustomerService
    {
        private delegate ValueTask<Customer> ReturningCustomerFunction();

        private async ValueTask<Customer> TryCatch(ReturningCustomerFunction returningCustomerFunction)
        {
            try
            {
                return await returningCustomerFunction();
            }
            catch (NullCustomerException nullCustomerException)
            {
                throw CreateAndLogValidationException(nullCustomerException);
            }
            catch (InvalidCustomerException invalidCustomerException)
            {
                throw CreateAndLogValidationException(invalidCustomerException);
            }
        }

        private CustomerValidationException CreateAndLogValidationException(Xeption exception)
        {
            var customerValidationException =
                new CustomerValidationException(exception);

            this.loggingBroker.LogError(customerValidationException);

            return customerValidationException;
        }
    }
}