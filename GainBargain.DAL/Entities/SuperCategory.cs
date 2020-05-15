using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace GainBargain.DAL.Entities
{
    public class SuperCategory
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public ICollection<Category> Categories { get; set; }
    }
}
