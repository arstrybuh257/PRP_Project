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

        public IEnumerable<ProductCache> GetProductsPerPage
            (int page, int pageSize, int superCategory, int? category, out int countProducts)
        {
            IEnumerable<ProductCache> products;
            if (category != null)
            {
                products = db.ProductsDemo.Where(p => p.CategoryId == category);
            }
            else
            {
                List<int> categoryIds = GetCategoryIdsBySuperCategory(superCategory);
                products = db.ProductsDemo.Where(p => categoryIds.Contains(p.CategoryId));
            }
            countProducts = products.Count();
            return products.Skip((page - 1) * pageSize).Take(pageSize).ToList();
        }

        public IEnumerable<ProductCache> GetProductsPerPage
           (int page, int pageSize, int superCategory, IEnumerable<int> categoriesIds, out int countProducts)
        {
            IEnumerable<ProductCache> products;
            if (categoriesIds.Count() > 0)
            {
                products = db.ProductsDemo
                .Where(p => categoriesIds.Contains(p.CategoryId));
                countProducts = products.Count();
                return products.Skip((page - 1) * pageSize).Take(pageSize).ToList();
            }
            List<int> categoryIds = GetCategoryIdsBySuperCategory(superCategory);
            products = db.ProductsDemo.Where(p => categoryIds.Contains(p.CategoryId));
            countProducts = products.Count();
            return products.Skip((page - 1) * pageSize).Take(pageSize).ToList();
        }

        private List<int> GetCategoryIdsBySuperCategory(int scId)
        {
            return db.Categories.Where(c => c.SuperCategoryId == scId).Select(c => c.Id).ToList();
        }
    }
}
