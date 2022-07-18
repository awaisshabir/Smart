using System;
using System.Linq;
using System.Threading.Tasks;
using EFxceptions.Models.Exceptions;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Smart.Api.Models.Customer;
using Smart.Api.Models.Customer.Exceptions;
using Xeptions;

namespace Smart.Api.Services.Foundations.Customer
{
    public partial class CustomersService
    {
        private delegate ValueTask<Customers> ReturningCustomersFunction();
        private delegate IQueryable<Customers> ReturningCustomerFunction();

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
            catch (DbUpdateException databaseUpdateException)
            {
                var failedCustomersStorageException =
                    new FailedCustomersStorageException(databaseUpdateException);

                throw CreateAndLogDependecyException(failedCustomersStorageException);
            }
            catch (Exception exception)
            {
                var failedCustomersServiceException =
                    new FailedCustomersServiceException(exception);

                throw CreateAndLogServiceException(failedCustomersServiceException);
            }
        }

        private IQueryable<Customers> TryCatch(ReturningCustomerFunction returningCustomerFunction)
        {
            try
            {
                return returningCustomerFunction();
            }
            catch (SqlException sqlException)
            {
                var failedCustomersStorageException =
                    new FailedCustomersStorageException(sqlException);
                throw CreateAndLogCriticalDependencyException(failedCustomersStorageException);
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

        private CustomersDependencyException CreateAndLogDependecyException(
            Xeption exception)
        {
            var customersDependencyException = new CustomersDependencyException(exception);
            this.loggingBroker.LogError(customersDependencyException);

            return customersDependencyException;
        }

        private CustomersServiceException CreateAndLogServiceException(
            Xeption exception)
        {
            var customersServiceException = new CustomersServiceException(exception);
            this.loggingBroker.LogError(customersServiceException);

            return customersServiceException;
        }
    }
}