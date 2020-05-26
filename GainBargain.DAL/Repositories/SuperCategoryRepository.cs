using GainBargain.DAL.EF;
using GainBargain.DAL.Entities;
using GainBargain.DAL.Interfaces;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace GainBargain.DAL.Repositories
{
    public class SuperCategoryRepository : Repository<SuperCategory>, ISuperCategoryRepository
    {
        public GainBargainContext gbContext => context as GainBargainContext;

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

    public class DbLogsRepository : Repository<DbLog>, IDbLogsRepository
    {
        public GainBargainContext db => context as GainBargainContext;

        public DbLogsRepository(GainBargainContext context) : base(context) { }

        /// <summary>
        /// Logs message to the database.
        /// </summary>
        /// <param name="code">Code of the event that had occured.</param>
        /// <param name="msg">Additional info to be added 
        /// (can be omitted)</param>
        public void Log(DbLog.LogCode code, string msg = null)
        {
            db.DbLogs.Add(new DbLog
            {
                Code = code,
                Info = msg,
                Time = DateTime.Now
            });
        }
    }
}
