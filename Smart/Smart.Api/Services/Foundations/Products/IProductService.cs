using System.Threading.Tasks;
using Smart.Api.Models.Products;

namespace Smart.Api.Services.Foundations.Products
{
    public interface IProductService
    {
        ValueTask<Product> AddProductAsync(Product product);
    }
}