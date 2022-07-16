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
                (Rule: IsInvalid(customer.UpdatedByUserId), Parameter: nameof(Customer.UpdatedByUserId)));
        }

        private static void ValidateCustomerIsNotNull(Customer customer)
        {
            if (customer is null)
            {
                throw new NullCustomerException();
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