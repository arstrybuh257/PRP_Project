using GainBargain.DAL.Entities;
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
        //public PageInfo PageInfo { get; set; }
        public Pager Pager { get; set; }
        public string SortOrder { get; set; } //none; asc; desc 

        public CatalogVM()
        {
            SelectedCategories = new List<int>();
            AvailableCategories = new List<Category>();
            SortOrder = "none";
        }

        //public void SetAvailableCategories(IEnumerable<Category> categories)
        //{
        //    AvailableCategories = categories.Select(c =>
        //    new SelectListItem
        //    {
        //        Text = c.Name,
        //        Value = c.Id.ToString()
        //    }) ;
        //}
    }
}