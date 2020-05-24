using System;
using System.Collections.Generic;
using GainBargain.DAL.Entities;

namespace GainBargain.WEB.Models
{
    public class UserProfileViewModel
    {
        public string Name { get; set; }
        public IEnumerable<FavoriteCategory> FavoriteCategories;
        public IEnumerable<FavoriteCategory> FavoriteProducts;
    }
}