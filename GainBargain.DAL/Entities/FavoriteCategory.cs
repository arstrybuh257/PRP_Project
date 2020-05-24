using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

namespace GainBargain.DAL.Entities
{
    [Table("FavoriteCategories")]
    public class FavoriteCategory
    {
        [Key]
        [Required, Column(Order = 0)]
        public int UserId;
        [Key, Column(Order = 1)]
        [Required]
        public int CategoryId;
        public virtual User User { get; set; }
        public virtual Category Category { get; set; }
    }
}
