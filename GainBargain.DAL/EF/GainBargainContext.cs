using GainBargain.DAL.Entities;
using System.Data.Entity;

namespace GainBargain.DAL.EF
{
    public class GainBargainContext : DbContext
    {
        public GainBargainContext() : base() { }

        DbSet<Product> Products { get; set; }
        DbSet<Category> Categories { get; set; }
        DbSet<Market> Markets { get; set; }
        DbSet<ParserSource> ParserSources { get; set; }
    }
}
