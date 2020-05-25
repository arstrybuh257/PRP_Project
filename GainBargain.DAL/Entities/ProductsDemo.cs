using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GainBargain.DAL.Entities
{
    /// <summary>
    /// Product entity based on view
    /// ProductDemo which has the property
    /// PrevPrice and does not have duplicates.
    /// </summary>
    [Table("ProductsDemo")]
    public class ProductsDemo
    {
        [Key]
        public int Id { set; get; }

        public string Name { set; get; }

        public string ImageUrl { set; get; }

        public float Price { set; get; }

        /// <summary>
        /// Previous price of this product.
        /// </summary>
        public float PrevPrice { set; get; }

        public int MarketId { set; get; }

        public int CategoryId { set; get; }
    }
}
