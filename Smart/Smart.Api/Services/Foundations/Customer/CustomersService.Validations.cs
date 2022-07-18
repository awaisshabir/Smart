using System;
using Smart.Api.Models.Customer;
using Smart.Api.Models.Customer.Exceptions;

namespace Smart.Api.Services.Foundations.Customer
{
    public partial class CustomersService
    {
        private void ValidateCustomersOnAdd(Customers customers)
        {
            ValidateCustomersIsNotNull(customers);

            Validate(
                (Rule: IsInvalid(customers.Id), Parameter: nameof(Customers.Id)),

                // TODO: Add any other required validation rules

                (Rule: IsInvalid(customers.CreatedDate), Parameter: nameof(Customers.CreatedDate)),
                (Rule: IsInvalid(customers.CreatedByUserId), Parameter: nameof(Customers.CreatedByUserId)),
                (Rule: IsInvalid(customers.UpdatedDate), Parameter: nameof(Customers.UpdatedDate)),
                (Rule: IsInvalid(customers.UpdatedByUserId), Parameter: nameof(Customers.UpdatedByUserId)),

                (Rule: IsNotSame(
                    firstDate: customers.UpdatedDate,
                    secondDate: customers.CreatedDate,
                    secondDateName: nameof(Customers.CreatedDate)),
                Parameter: nameof(Customers.UpdatedDate)),

                (Rule: IsNotSame(
                    firstId: customers.UpdatedByUserId,
                    secondId: customers.CreatedByUserId,
                    secondIdName: nameof(Customers.CreatedByUserId)),
                Parameter: nameof(Customers.UpdatedByUserId)),

                (Rule: IsNotRecent(customers.CreatedDate), Parameter: nameof(Customers.CreatedDate)));
        }

        private void ValidateCustomersOnModify(Customers customers)
        {
            ValidateCustomersIsNotNull(customers);

            Validate(
                (Rule: IsInvalid(customers.Id), Parameter: nameof(Customers.Id)),

                // TODO: Add any other required validation rules

                (Rule: IsInvalid(customers.CreatedDate), Parameter: nameof(Customers.CreatedDate)),
                (Rule: IsInvalid(customers.CreatedByUserId), Parameter: nameof(Customers.CreatedByUserId)),
                (Rule: IsInvalid(customers.UpdatedDate), Parameter: nameof(Customers.UpdatedDate)),
                (Rule: IsInvalid(customers.UpdatedByUserId), Parameter: nameof(Customers.UpdatedByUserId)));
        }

        public void ValidateCustomersId(Guid customersId) =>
            Validate((Rule: IsInvalid(customersId), Parameter: nameof(Customers.Id)));

        private static void ValidateStorageCustomers(Customers maybeCustomers, Guid customersId)
        {
            if (maybeCustomers is null)
            {
                throw new NotFoundCustomersException(customersId);
            }
        }

        private static void ValidateCustomersIsNotNull(Customers customers)
        {
            if (customers is null)
            {
                throw new NullCustomersException();
            }
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
            var invalidCustomersException = new InvalidCustomersException();

            foreach ((dynamic rule, string parameter) in validations)
            {
                if (rule.Condition)
                {
                    invalidCustomersException.UpsertDataList(
                        key: parameter,
                        value: rule.Message);
                }
            }

            invalidCustomersException.ThrowIfContainsErrors();
        }
    }
}