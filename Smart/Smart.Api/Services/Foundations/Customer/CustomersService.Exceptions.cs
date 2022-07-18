using System.Threading.Tasks;
using EFxceptions.Models.Exceptions;
using Microsoft.Data.SqlClient;
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
            catch (SqlException sqlException)
            {
                var failedCustomersStorageException =
                    new FailedCustomersStorageException(sqlException);

                throw CreateAndLogCriticalDependencyException(failedCustomersStorageException);
            }
            catch (DuplicateKeyException duplicateKeyException)
            {
                var alreadyExistsCustomersException =
                    new AlreadyExistsCustomersException(duplicateKeyException);

                throw CreateAndLogDependencyValidationException(alreadyExistsCustomersException);
            }
            catch (ForeignKeyConstraintConflictException foreignKeyConstraintConflictException)
            {
                var invalidCustomersReferenceException =
                    new InvalidCustomersReferenceException(foreignKeyConstraintConflictException);

                throw CreateAndLogDependencyValidationException(invalidCustomersReferenceException);
            }
        }

        private CustomersValidationException CreateAndLogValidationException(Xeption exception)
        {
            var customersValidationException =
                new CustomersValidationException(exception);

            this.loggingBroker.LogError(customersValidationException);

            return customersValidationException;
        }

        private CustomersDependencyException CreateAndLogCriticalDependencyException(Xeption exception)
        {
            var customersDependencyException = new CustomersDependencyException(exception);
            this.loggingBroker.LogCritical(customersDependencyException);

            return customersDependencyException;
        }

        private CustomersDependencyValidationException CreateAndLogDependencyValidationException(Xeption exception)
        {
            var customersDependencyValidationException =
                new CustomersDependencyValidationException(exception);

            this.loggingBroker.LogError(customersDependencyValidationException);

            return customersDependencyValidationException;
        }
    }
}