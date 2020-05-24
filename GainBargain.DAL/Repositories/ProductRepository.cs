using GainBargain.DAL.EF;
using GainBargain.DAL.Entities;
using GainBargain.DAL.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace GainBargain.DAL.Repositories
{
    public class ProductRepository : Repository<Product>, IProductRepository
    {
        public GainBargainContext gbContext => context as GainBargainContext;

        public ProductRepository(GainBargainContext context) : base(context) { }

        public IEnumerable<Product> ProductsWithSameSuperCategory(int id)
        {
            var products = gbContext.Products.Where(p => p.Category.SuperCategoryId == id).ToList();
            return products;
        }
    }
}
