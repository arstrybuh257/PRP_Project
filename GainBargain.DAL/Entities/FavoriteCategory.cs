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
        public int UserId { get; set; }
        [Key, Column(Order = 1)]
        [Required]
        public int CategoryId { get; set; }
        public User User { get; set; }
        public Category Category { get; set; }
    }
}
