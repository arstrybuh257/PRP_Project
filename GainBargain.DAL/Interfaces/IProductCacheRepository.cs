using GainBargain.DAL.Entities;
using System.Collections.Generic;

namespace GainBargain.DAL.Interfaces
{
    public interface IProductCacheRepository : IRepository<ProductCache>
    {
        IEnumerable<ProductCache> GetTopProducts(int count);

        IEnumerable<ProductCache> GetTopProducts(int count, int categoryId);
    }
}
