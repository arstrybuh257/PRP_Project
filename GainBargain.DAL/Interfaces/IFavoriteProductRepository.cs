using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GainBargain.DAL.Entities;

namespace GainBargain.DAL.Interfaces
{
    public interface IFavoriteProductRepository
    {
        void AddFavoriteProduct(FavoriteProduct category);
        void RemoveFromFavoriteProducts(int productId, string userName);
        IEnumerable<FavoriteProduct> FindByUserName(string userName);
        bool IsFavorite(int productId, string userName);
    }
}
