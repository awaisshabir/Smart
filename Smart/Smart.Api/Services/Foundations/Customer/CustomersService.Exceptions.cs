using System.Threading.Tasks;
using Smart.Api.Models.Customer;
using Smart.Api.Models.Customer.Exceptions;
using Xeptions;

namespace Smart.Api.Services.Foundations.Customer
{
    public partial class CustomersService
    {
        private delegate ValueTask<Customers> ReturningCustomersFunction();

        private async ValueTask<Customers> TryCatch(ReturningCustomersFunction returningCustomersFunction)
        {
            try
            {
                return await returningCustomersFunction();
            }
            catch (NullCustomersException nullCustomersException)
            {
                throw CreateAndLogValidationException(nullCustomersException);
            }
            catch (InvalidCustomersException invalidCustomersException)
            {
                throw CreateAndLogValidationException(invalidCustomersException);
            }
        }

        private CustomersValidationException CreateAndLogValidationException(Xeption exception)
        {
            var customersValidationException =
                new CustomersValidationException(exception);

            this.loggingBroker.LogError(customersValidationException);

            return customersValidationException;
        }
    }
}