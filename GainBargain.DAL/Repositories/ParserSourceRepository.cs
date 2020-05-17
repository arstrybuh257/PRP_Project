using GainBargain.DAL.EF;
using GainBargain.DAL.Entities;
using GainBargain.DAL.Interfaces;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace GainBargain.DAL.Repositories
{
    public class ParserSourceRepository : Repository<ParserSource>, IParserSourceRepository
    {
        public GainBargainContext gbContext
        {
            get { return context as GainBargainContext; }
        }

        public ParserSourceRepository(GainBargainContext context) : base(context) { }

        public IEnumerable<SuperCategory> GetSuperCategoriesWithCategories()
        {
            return gbContext.SuperCategories.Include(sc => sc.Categories).ToList();
        }
        public IEnumerable<ParserSource> GetAllParserSources()
        {
            return gbContext.ParserSources
                .Include(ps => ps.Market)
                .Include(ps => ps.Category)
                .ToList();
        }
    }
}
