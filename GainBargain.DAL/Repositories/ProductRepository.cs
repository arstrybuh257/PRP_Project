using GainBargain.DAL.EF;
using GainBargain.DAL.Entities;
using GainBargain.DAL.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GainBargain.DAL.Repositories
{
    public class ProductRepository : Repository<Product>, IProductRepository
    {
        public GainBargainContext gbContext
        {
            get { return context as GainBargainContext; }
        }

        public ProductRepository(GainBargainContext context) : base(context) { }

        public IEnumerable<Product> ProductsWithSameSuperCategory(int id)
        {
            var products = gbContext.Products.Where(p => p.Category.SuperCategoryId == id).ToList();
            return products;
        }

        public IEnumerable<Product> GetProductsPerPage
            (int page, int pageSize, int superCategory, int? category, out int countProducts)
        {
            IEnumerable<Product> products;
            if (category != null)
            {
                products = gbContext.Products.Where(p => p.CategoryId == category);
            }
            else
            {
                products = gbContext.Products.Where(p => p.Category.SuperCategoryId == superCategory);
            }
            countProducts = products.Count();
            return products.Skip((page - 1) * pageSize).Take(pageSize).ToList();
        }

        public IEnumerable<Product> GetProductsPerPage
           (int page, int pageSize, int superCategory, IEnumerable<int> categoriesIds, out int countProducts)
        {
            IEnumerable<Product> products;
            if (categoriesIds.Count() > 0)
            {
                products = gbContext.Products
                .Where(p => categoriesIds.Contains(p.CategoryId));
                countProducts = products.Count();
                return products.Skip((page - 1) * pageSize).Take(pageSize).ToList();
            }
            products = gbContext.Products.Where(p => p.Category.SuperCategoryId == superCategory);
            countProducts = products.Count();
            return products.Skip((page - 1) * pageSize).Take(pageSize).ToList();
        }

        public int Count(int superCategory, int? category)
        {
            if (category != null)
            {
                return gbContext.Products.Where(p => p.CategoryId == category).Count();
            }
            return gbContext.Products.Where(p => p.Category.SuperCategoryId == superCategory).Count();
        }
    }
}
