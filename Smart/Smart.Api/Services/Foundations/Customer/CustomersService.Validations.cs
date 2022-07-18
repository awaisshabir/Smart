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
                (Rule: IsInvalid(customers.UpdatedByUserId), Parameter: nameof(Customers.UpdatedByUserId)));
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