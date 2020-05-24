using System.Collections.Generic;
using System.Linq;
using GainBargain.DAL.EF;
using GainBargain.DAL.Entities;
using GainBargain.DAL.Interfaces;

namespace GainBargain.DAL.Repositories
{
    public class FavoriteCategoriesRepository : Repository<FavoriteCategory>, IFavoriteCategoriesRepository
    {

        public GainBargainContext gbContext
        {
            get { return context as GainBargainContext; }
        }
        public FavoriteCategoriesRepository(GainBargainContext context) : base(context) { }
        public IEnumerable<FavoriteCategory> GetFavoriteCategories()
        {
           return gbContext.FavoriteCategories.Include("Category").AsNoTracking().ToList();
        }

        public void AddFavoriteCategory(FavoriteCategory category)
        {
            throw new System.NotImplementedException();
        }

        public void RemoveFromFavoriteCategory(int categoryId)
        {
            throw new System.NotImplementedException();
        }

        public IEnumerable<FavoriteCategory> FindByUserId(int userId)
        {
            return gbContext.FavoriteCategories.Include("Category").AsNoTracking().Where(x=>x.UserId == userId).ToList();
        }
    }
}
