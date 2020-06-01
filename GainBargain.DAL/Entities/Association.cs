using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GainBargain.DAL.Entities
{
    [Table("Association")]
    public class Association
    {
        [Key]
        [Column(Order = 0)]
        public int ProductId { get; set; }
        [Key]
        [Column(Order = 1)]
        public int AccosiationId { get; set; }

        [ForeignKey("ProductId")]
        public Product Product { get; set; }
        [ForeignKey("AccosiationId")]
        public ProductCache ProductCache { get; set; }
    }
}
