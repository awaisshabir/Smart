using System;
using Smart.Api.Models.Customers;
using Smart.Api.Models.Customers.Exceptions;

namespace Smart.Api.Services.Foundations.Customers
{
    public partial class CustomerService
    {
        private void ValidateCustomerOnAdd(Customer customer)
        {
            ValidateCustomerIsNotNull(customer);

            Validate(
                (Rule: IsInvalid(customer.Id), Parameter: nameof(Customer.Id)),

                // TODO: Add any other required validation rules

                (Rule: IsInvalid(customer.CreatedDate), Parameter: nameof(Customer.CreatedDate)),
                (Rule: IsInvalid(customer.CreatedByUserId), Parameter: nameof(Customer.CreatedByUserId)),
                (Rule: IsInvalid(customer.UpdatedDate), Parameter: nameof(Customer.UpdatedDate)),
                (Rule: IsInvalid(customer.UpdatedByUserId), Parameter: nameof(Customer.UpdatedByUserId)),

                (Rule: IsNotSame(
                    firstDate: customer.UpdatedDate,
                    secondDate: customer.CreatedDate,
                    secondDateName: nameof(Customer.CreatedDate)),
                Parameter: nameof(Customer.UpdatedDate)),

                (Rule: IsNotSame(
                    firstId: customer.UpdatedByUserId,
                    secondId: customer.CreatedByUserId,
                    secondIdName: nameof(Customer.CreatedByUserId)),
                Parameter: nameof(Customer.UpdatedByUserId)),

                (Rule: IsNotRecent(customer.CreatedDate), Parameter: nameof(Customer.CreatedDate)));
        }

        private void ValidateCustomerOnModify(Customer customer)
        {
            ValidateCustomerIsNotNull(customer);

            Validate(
                (Rule: IsInvalid(customer.Id), Parameter: nameof(Customer.Id)),

                // TODO: Add any other required validation rules

                (Rule: IsInvalid(customer.CreatedDate), Parameter: nameof(Customer.CreatedDate)),
                (Rule: IsInvalid(customer.CreatedByUserId), Parameter: nameof(Customer.CreatedByUserId)),
                (Rule: IsInvalid(customer.UpdatedDate), Parameter: nameof(Customer.UpdatedDate)),
                (Rule: IsInvalid(customer.UpdatedByUserId), Parameter: nameof(Customer.UpdatedByUserId)),

                (Rule: IsSame(
                    firstDate: customer.UpdatedDate,
                    secondDate: customer.CreatedDate,
                    secondDateName: nameof(Customer.CreatedDate)),
                Parameter: nameof(Customer.UpdatedDate)),

                (Rule: IsNotRecent(customer.UpdatedDate), Parameter: nameof(customer.UpdatedDate)));
        }

        public void ValidateCustomerId(Guid customerId) =>
            Validate((Rule: IsInvalid(customerId), Parameter: nameof(Customer.Id)));

        private static void ValidateStorageCustomer(Customer maybeCustomer, Guid customerId)
        {
            if (maybeCustomer is null)
            {
                throw new NotFoundCustomerException(customerId);
            }
        }

        private static void ValidateCustomerIsNotNull(Customer customer)
        {
            if (customer is null)
            {
                throw new NullCustomerException();
            }
        }

        private static void ValidateAgainstStorageCustomerOnModify(Customer inputCustomer, Customer storageCustomer)
        {
            Validate(
                (Rule: IsNotSame(
                    firstDate: inputCustomer.CreatedDate,
                    secondDate: storageCustomer.CreatedDate,
                    secondDateName: nameof(Customer.CreatedDate)),
                Parameter: nameof(Customer.CreatedDate)),

                (Rule: IsNotSame(
                    firstId: inputCustomer.CreatedByUserId,
                    secondId: storageCustomer.CreatedByUserId,
                    secondIdName: nameof(Customer.CreatedByUserId)),
                Parameter: nameof(Customer.CreatedByUserId)),

                (Rule: IsSame(
                    firstDate: inputCustomer.UpdatedDate,
                    secondDate: storageCustomer.UpdatedDate,
                    secondDateName: nameof(Customer.UpdatedDate)),
                Parameter: nameof(Customer.UpdatedDate)));
        }

        private static dynamic IsInvalid(Guid id) => new
        {
            Condition = id == Guid.Empty,
            Message = "Id is required"
        };

        private static dynamic IsInvalid(DateTimeOffset date) => new
        {
            Condition = date == default,
            Message = "Date is required"
        };

        private static dynamic IsSame(
            DateTimeOffset firstDate,
            DateTimeOffset secondDate,
            string secondDateName) => new
            {
                Condition = firstDate == secondDate,
                Message = $"Date is the same as {secondDateName}"
            };

        private static dynamic IsNotSame(
            DateTimeOffset firstDate,
            DateTimeOffset secondDate,
            string secondDateName) => new
            {
                Condition = firstDate != secondDate,
                Message = $"Date is not the same as {secondDateName}"
            };

        private static dynamic IsNotSame(
            Guid firstId,
            Guid secondId,
            string secondIdName) => new
            {
                Condition = firstId != secondId,
                Message = $"Id is not the same as {secondIdName}"
            };

        private dynamic IsNotRecent(DateTimeOffset date) => new
        {
            Condition = IsDateNotRecent(date),
            Message = "Date is not recent"
        };

        private bool IsDateNotRecent(DateTimeOffset date)
        {
            DateTimeOffset currentDateTime =
                this.dateTimeBroker.GetCurrentDateTimeOffset();

            TimeSpan timeDifference = currentDateTime.Subtract(date);
            TimeSpan oneMinute = TimeSpan.FromMinutes(1);

            return timeDifference.Duration() > oneMinute;
        }

        private static void Validate(params (dynamic Rule, string Parameter)[] validations)
        {
            var invalidCustomerException = new InvalidCustomerException();

            foreach ((dynamic rule, string parameter) in validations)
            {
                if (rule.Condition)
                {
                    invalidCustomerException.UpsertDataList(
                        key: parameter,
                        value: rule.Message);
                }
            }

            invalidCustomerException.ThrowIfContainsErrors();
        }
    }
}