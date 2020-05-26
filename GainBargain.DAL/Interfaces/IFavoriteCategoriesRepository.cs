using System.Collections.Generic;
using GainBargain.DAL.Entities;

namespace GainBargain.DAL.Interfaces
{
    public interface IFavoriteCategoriesRepository
    {
        IEnumerable<FavoriteCategory> GetFavoriteCategories();
        void AddFavoriteCategory(FavoriteCategory category);
        void RemoveFromFavoriteCategory(int categoryId, string userName);
        IEnumerable<FavoriteCategory> FindByUserName(string userName);
    }
}
