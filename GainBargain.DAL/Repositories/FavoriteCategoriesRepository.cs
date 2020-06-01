using System;
using System.Collections.Generic;
using System.Linq;
using GainBargain.DAL.EF;
using GainBargain.DAL.Entities;
using GainBargain.DAL.Interfaces;

namespace GainBargain.DAL.Repositories
{
    public class FavoriteCategoriesRepository : IFavoriteCategoriesRepository
    {
        private GainBargainContext db;
        public FavoriteCategoriesRepository()
        {
            db = new GainBargainContext();
        }
        public IEnumerable<FavoriteCategory> GetFavoriteCategories()
        {
           return db.FavoriteCategories.Include("Category").AsNoTracking().ToList();
        }

        public void AddFavoriteCategory(FavoriteCategory category)
        {
            db.FavoriteCategories.Add(category);
            db.SaveChanges();
        }

        public void RemoveFromFavoriteCategory(int categoryId, string userName)
        {
            var entity = db.FavoriteCategories
                .FirstOrDefault(x => x.CategoryId == categoryId && x.User.Email == userName);
            if (entity != null)
            {
                db.FavoriteCategories.Remove(entity);
                db.SaveChanges();
            }
        }

        public IEnumerable<FavoriteCategory> FindByUserName(string userName)
        {
            return db.FavoriteCategories.Include("Category").AsNoTracking().Where(x=>x.User.Email == userName).ToList();
        }
    }
}
