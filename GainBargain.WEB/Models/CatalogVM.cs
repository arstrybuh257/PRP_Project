﻿using GainBargain.DAL.Entities;
using System.Collections.Generic;

namespace GainBargain.WEB.Models
{
    public class CatalogVM
    {
        public int SuperCategoryId { get; set; }
        public string SuperCategoryName { get; set; }
        public IEnumerable<Product> Products { get; set; }
        //public IEnumerable<Market> Markets { get; set; }
        public IList<int> SelectedCategories { get; set; }
        public IEnumerable<Category> AvailableCategories { get; set; }
        public Pager Pager { get; set; }

        public int CountProducts { get; set; }
        public bool? SortOrder { get; set; } //null; asc -- true; desc -- false

        public CatalogVM()
        {
            SelectedCategories = new List<int>();
            AvailableCategories = new List<Category>();
        }
    }
}