using System;
using Smart.Api.Models.Products;
using Smart.Api.Models.Products.Exceptions;

namespace Smart.Api.Services.Foundations.Products
{
    public partial class ProductService
    {
        private void ValidateProductOnAdd(Product product)
        {
            ValidateProductIsNotNull(product);

            Validate(
                (Rule: IsInvalid(product.Id), Parameter: nameof(Product.Id)),

                // TODO: Add any other required validation rules

                (Rule: IsInvalid(product.CreatedDate), Parameter: nameof(Product.CreatedDate)),
                (Rule: IsInvalid(product.CreatedByUserId), Parameter: nameof(Product.CreatedByUserId)),
                (Rule: IsInvalid(product.UpdatedDate), Parameter: nameof(Product.UpdatedDate)),
                (Rule: IsInvalid(product.UpdatedByUserId), Parameter: nameof(Product.UpdatedByUserId)));
        }

        private static void ValidateProductIsNotNull(Product product)
        {
            if (product is null)
            {
                throw new NullProductException();
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
            var invalidProductException = new InvalidProductException();

            foreach ((dynamic rule, string parameter) in validations)
            {
                if (rule.Condition)
                {
                    invalidProductException.UpsertDataList(
                        key: parameter,
                        value: rule.Message);
                }
            }

            invalidProductException.ThrowIfContainsErrors();
        }
    }
}