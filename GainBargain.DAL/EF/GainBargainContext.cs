﻿using System.Data;
using GainBargain.DAL.Entities;
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
        public DbSet<SuperCategory> SuperCategories { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }
        public DbSet<FavoriteCategory> FavoriteCategories { get; set; }
        public DbSet<Association> Associations { get; set; }

        /// <summary>
        /// Perfect for displaying product entity.
        /// </summary>
        public DbSet<ProductCache> ProductsDemo { get; set; }

        /// <summary>
        /// Used to write and read logs about system's activity.
        /// </summary>
        public DbSet<DbLog> DbLogs { get; set; }


        public DbSet<FavoriteProduct> FavoriteProducts { get; set; }
    }
}
