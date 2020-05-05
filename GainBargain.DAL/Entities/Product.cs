using GainBargain.Parser.Interfaces;
using System;
using System.ComponentModel.DataAnnotations;

namespace GainBargain.DAL.Entities
{
    public class Product : IParserOutput<float>
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
    }
}
