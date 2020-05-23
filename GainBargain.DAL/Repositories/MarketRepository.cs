using GainBargain.DAL.EF;
using GainBargain.DAL.Entities;
using GainBargain.DAL.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GainBargain.DAL.Repositories
{
    public class MarketRepository : Repository<Market>, IMarketRepository
    {
        public GainBargainContext gbContext
        {
            get { return context as GainBargainContext; }
        }
        public MarketRepository(GainBargainContext context) : base(context) { }
        public IEnumerable<Market> FindMarkets(IEnumerable<int> ids)
        {
            var markets = gbContext.Products
                .Where(p => ids.Contains(p.Id))
                .Select(p => p.Market)
                .Distinct()
                .ToList();
            return markets;
        }
        public IEnumerable<Market> FindMarketsBySuperCategory(int id)
        {
            var markets = gbContext.Products
                .Where(p => p.Category.SuperCategoryId == id)
                .Select(p => p.Market)
                .Distinct()
                .ToList();
            return markets;
        }
    }
}
