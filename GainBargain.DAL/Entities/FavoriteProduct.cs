using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GainBargain.DAL.Entities
{
    [Table("FavoriteProducts")]
    public class FavoriteProduct
    {
        [Key]
        [Column(Order = 0)]
        public int UserId;
        [Key]
        [Column(Order = 1)]
        public int ProductId;
    }
}
