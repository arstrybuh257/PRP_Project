﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
    }
}
