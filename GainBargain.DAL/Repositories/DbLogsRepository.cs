using GainBargain.DAL.EF;
using GainBargain.DAL.Entities;
using GainBargain.DAL.Interfaces;
using System;

namespace GainBargain.DAL.Repositories
{
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
            db.SaveChanges();
        }
    }
}
