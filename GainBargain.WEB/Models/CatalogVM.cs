using GainBargain.DAL.Entities;
using System.Collections.Generic;

namespace GainBargain.WEB.Models
{
    public class CatalogVM
    {
        public int SuperCategoryId { get; set; }
        public string SuperCategoryName { get; set; }
        //public IEnumerable<Product> Products { get; set; }
        public IEnumerable<Market> Markets { get; set; }
        public IEnumerable<Category> Categories { get; set; }
    }
}