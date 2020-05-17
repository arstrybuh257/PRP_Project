using GainBargain.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GainBargain.DAL.Interfaces
{
    public interface ISuperCategoryRepository : IRepository<SuperCategory>
    {
        IEnumerable<SuperCategory> GetSuperCategoriesWithCategories(); 
    }
}
