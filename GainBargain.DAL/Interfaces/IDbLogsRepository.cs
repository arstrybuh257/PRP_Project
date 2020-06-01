using GainBargain.DAL.Entities;

namespace GainBargain.DAL.Interfaces
{
    public interface IDbLogsRepository : IRepository<DbLog>
    {
        void Log(DbLog.LogCode code, string msg);
    }
}
