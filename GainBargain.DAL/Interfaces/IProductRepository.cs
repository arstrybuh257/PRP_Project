using GainBargain.DAL.Entities;
using System.Collections.Generic;

namespace GainBargain.DAL.Interfaces
{
    public interface IProductRepository : IRepository<Product>
    {
        IEnumerable<Product> ProductsWithSameSuperCategory(int id);
        IEnumerable<Product> GetProductsPerPage
            (int page, int pageSize, int superCategory, int? category, out int countProducts);

        IEnumerable<Product> GetProductsPerPage
          (int page, int pageSize, int superCategory, IEnumerable<int> categoriesIds, out int countProducts);
        int Count(int superCategory, int? category);
    }
}
