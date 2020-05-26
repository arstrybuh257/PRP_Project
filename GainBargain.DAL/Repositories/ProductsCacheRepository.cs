using GainBargain.DAL.EF;
using GainBargain.DAL.Entities;
using GainBargain.DAL.Interfaces;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace GainBargain.DAL.Repositories
{
    /// <summary>
    /// Basic service for products demonstration entity.
    /// </summary>
    public class ProductsCacheRepository : Repository<ProductCache>, IProductCacheRepository
    {
        /// <summary>
        /// Just another slap in the performance's face.
        /// </summary>
        public GainBargainContext db => context as GainBargainContext;

        public ProductsCacheRepository(GainBargainContext context) : base(context) { }

        public IEnumerable<ProductCache> GetTopProducts(int count)
        {
            db.Database.Log = new System.Action<string>(f => System.Diagnostics.Debug.WriteLine(f));
            var q = db.ProductsDemo
                .OrderBy(p => (db.ProductsDemo
                    .Where(avgp => avgp.CategoryId == p.CategoryId)
                    .Average(avgp => avgp.PrevPrice - avgp.Price)) - (p.PrevPrice - p.Price))
                .Take(count)
                .AsNoTracking();

            return q;
        }

        public IEnumerable<ProductCache> GetTopProducts(int count, int categoryId)
        {
            return db.ProductsDemo
                .Where(p => p.CategoryId == categoryId)
                .OrderByDescending(p => (db.ProductsDemo
                    .Where(avgp => avgp.CategoryId == categoryId)
                    .Average(avgp => avgp.PrevPrice - avgp.Price)) - (p.PrevPrice - p.Price))
                .Take(count)
                .AsNoTracking();
        }
    }
}
