using GainBargain.DAL.Entities;
using System.Collections.Generic;

namespace GainBargain.DAL.Interfaces
{
    public interface ISuperCategoryRepository : IRepository<SuperCategory>
    {
        IEnumerable<SuperCategory> GetSuperCategoriesWithCategories();
        IEnumerable<SuperCategory> FindSuperCategoriesWithCategories(int id);
    }
}
