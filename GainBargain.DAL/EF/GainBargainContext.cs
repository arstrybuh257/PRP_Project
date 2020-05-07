﻿using GainBargain.DAL.Entities;
using System.Data.Entity;

namespace GainBargain.DAL.EF
{
    public class GainBargainContext : DbContext
    {
        public GainBargainContext() : base() { }

        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Market> Markets { get; set; }
        public DbSet<ParserSource> ParserSources { get; set; }
    }
}
