using GainBargain.DAL.EF;
using GainBargain.DAL.Entities;
using GainBargain.DAL.Interfaces;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace GainBargain.DAL.Repositories
{
    public class SuperCategoryRepository : Repository<SuperCategory>, ISuperCategoryRepository
    {
        public GainBargainContext gbContext
        {
            get { return context as GainBargainContext; }
        }

        public SuperCategoryRepository(GainBargainContext context) : base(context) { }

        public IEnumerable<SuperCategory> GetAllSuperCategoriesWithCategories()
        {
            return gbContext.SuperCategories.Include(sc => sc.Categories).ToList();
        }

        public SuperCategory GetSuperCategoryWithCategories(int id)
        {
            return gbContext.SuperCategories.Where(sc => sc.Id == id)
                .Include(sc => sc.Categories).FirstOrDefault();
        }
    }
}
