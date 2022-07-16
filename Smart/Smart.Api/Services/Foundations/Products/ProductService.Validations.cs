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
                (Rule: IsInvalid(product.UpdatedByUserId), Parameter: nameof(Product.UpdatedByUserId)),

                (Rule: IsNotSame(
                    firstDate: product.UpdatedDate,
                    secondDate: product.CreatedDate,
                    secondDateName: nameof(Product.CreatedDate)),
                Parameter: nameof(Product.UpdatedDate)),

                (Rule: IsNotSame(
                    firstId: product.UpdatedByUserId,
                    secondId: product.CreatedByUserId,
                    secondIdName: nameof(Product.CreatedByUserId)),
                Parameter: nameof(Product.UpdatedByUserId)),

                (Rule: IsNotRecent(product.CreatedDate), Parameter: nameof(Product.CreatedDate)));
        }

        private void ValidateProductOnModify(Product product)
        {
            ValidateProductIsNotNull(product);

            Validate(
                (Rule: IsInvalid(product.Id), Parameter: nameof(Product.Id)),

                // TODO: Add any other required validation rules

                (Rule: IsInvalid(product.CreatedDate), Parameter: nameof(Product.CreatedDate)),
                (Rule: IsInvalid(product.CreatedByUserId), Parameter: nameof(Product.CreatedByUserId)),
                (Rule: IsInvalid(product.UpdatedDate), Parameter: nameof(Product.UpdatedDate)),
                (Rule: IsInvalid(product.UpdatedByUserId), Parameter: nameof(Product.UpdatedByUserId)),

                (Rule: IsSame(
                    firstDate: product.UpdatedDate,
                    secondDate: product.CreatedDate,
                    secondDateName: nameof(Product.CreatedDate)),
                Parameter: nameof(Product.UpdatedDate)),

                (Rule: IsNotRecent(product.UpdatedDate), Parameter: nameof(product.UpdatedDate)));
        }

        public void ValidateProductId(Guid productId) =>
            Validate((Rule: IsInvalid(productId), Parameter: nameof(Product.Id)));

        private static void ValidateStorageProduct(Product maybeProduct, Guid productId)
        {
            if (maybeProduct is null)
            {
                throw new NotFoundProductException(productId);
            }
        }

        private static void ValidateProductIsNotNull(Product product)
        {
            if (product is null)
            {
                throw new NullProductException();
            }
        }

        private static void ValidateAgainstStorageProductOnModify(Product inputProduct, Product storageProduct)
        {
            Validate(
                (Rule: IsNotSame(
                    firstDate: inputProduct.CreatedDate,
                    secondDate: storageProduct.CreatedDate,
                    secondDateName: nameof(Product.CreatedDate)),
                Parameter: nameof(Product.CreatedDate)));
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