using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using GainBargain.DAL.EF;
using GainBargain.DAL.Entities;
using GainBargain.DAL.Interfaces;

namespace GainBargain.DAL.Repositories
{
    public class FavoriteProductRepository : IFavoriteProductRepository
    {
        private GainBargainContext db;
        public FavoriteProductRepository()
        {
            db = new GainBargainContext();
        }
        public void AddFavoriteProduct(FavoriteProduct product)
        {
            db.FavoriteProducts.Add(product);
            db.SaveChanges();
        }

        public IEnumerable<FavoriteProduct> FindByUserName(string userName)
        {
            return db.FavoriteProducts.Where(x=>x.User.Email == userName).Include("Product").ToList();
        }

        public bool IsFavorite(int productId, string userName)
        {
            return db.FavoriteProducts.Any(x => x.ProductId == productId && x.User.Email == userName);
        }

        public void RemoveFromFavoriteProducts(int productId, string userName)
        {
            var entity = db.FavoriteProducts
                .FirstOrDefault(x => x.ProductId == productId && x.User.Email == userName);
            if (entity != null)
            {
                db.FavoriteProducts.Remove(entity);
                db.SaveChanges();
            }
        }

        public List<List<int>> GetAllTransactions()
        {
            List<List<int>> result = new List<List<int>>();

            var groups = db.FavoriteProducts.GroupBy(x=>x.UserId);

            foreach (var group in groups)
            {
                List<int> list = new List<int>();
                foreach (var pr in group)
                {
                    list.Add(pr.ProductId);
                }
                result.Add(list);
            }

            return result;
        }
    }
}
