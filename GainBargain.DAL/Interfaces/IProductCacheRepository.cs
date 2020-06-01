using GainBargain.DAL.Entities;
using System.Collections.Generic;

namespace GainBargain.DAL.Interfaces
{
    public interface IProductCacheRepository : IRepository<ProductCache>
    {
        IEnumerable<ProductCache> GetTopProducts(int count);

        IEnumerable<ProductCache> GetTopProducts(int count, int categoryId);

        IEnumerable<ProductCache> GetProductsPerPage
            (int page, int pageSize, int superCategory, int? category, out int countProducts);

        IEnumerable<ProductCache> GetProductsPerPage
          (int page, int pageSize, int superCategory, IEnumerable<int> categoriesIds, out int countProducts);

        IEnumerable<ProductCache> SearchProduct(int page, int pageSize, string text, out int countProducts);
    }
}
