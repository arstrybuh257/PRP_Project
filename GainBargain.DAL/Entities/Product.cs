using System;
using System.ComponentModel.DataAnnotations;

namespace GainBargain.DAL.Entities
{
    public class Product
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public float Price { get; set; }
        [Required]
        public DateTime UploadTime { get; set; }
        public string ImageUrl { set; get; }


        public int CategoryId { get; set; }
        public virtual Category Category { get; set; }
        public int MarketId { get; set; }
        public virtual Market Market { get; set; } 

        /// <summary>
        /// Invokes every time the Product is parsed in order
        /// to change product image's relative URL to the absolute one.
        /// You can create different mathods with the same signature in this
        /// class in order to add new post processing or you can use this method as well.
        /// </summary>
        /// <param name="obj">Instance to post update.</param>
        /// <param name="input">Input given to parse this object.</param>
        public static void PostParsingAction(Product obj, ParserSource input)
        {
            // Get website domain name
            var pageHost = new Uri(input.Url).Host;

            // Image Url is domain name + relative path from src attribute
            obj.ImageUrl = $"{pageHost}/{obj.ImageUrl}";           
        }
    }
}
