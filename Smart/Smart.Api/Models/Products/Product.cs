using System;

namespace Smart.Api.Models.Products
{
    public class Product
    {
        public Guid Id { get; set; }

        // TODO:  Add your properties here

        public DateTimeOffset BirthDate { get; set; }
        public Guid CreatedByUserId { get; set; }
        public DateTimeOffset CreatedDate { get; set; }
        public Guid UpdatedByUserId { get; set; }
        public DateTimeOffset UpdatedDate { get; set; }
    }
}
