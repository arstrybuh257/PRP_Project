using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GainBargain.DAL.Entities
{
    [Table("FavoriteCategories")]
    public class FavoriteCategory
    {
        [Key]
        [Column(Order = 0)]
        public int UserId;
        [Key]
        [Column(Order = 1)]
        public int CategoryId;
    }
}
