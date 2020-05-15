using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace GainBargain.DAL.Entities
{
    public class Category
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }

        public int SuperCategoryId { get; set; }

        public SuperCategory SuperCategory { get; set; }

        public virtual ICollection<Product> Products { get; set; }
    }
}
