using GainBargain.DAL.Entities;
using System.Collections.Generic;

namespace GainBargain.DAL.Interfaces
{
    public interface IMarketRepository : IRepository<Market>
    {
        IEnumerable<Market> FindMarkets(IEnumerable<int> ids);
        IEnumerable<Market> FindMarketsBySuperCategory(int id);
    }
}
