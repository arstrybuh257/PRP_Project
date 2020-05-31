using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GainBargain.WEB.Models
{
    public class ProductVM
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string ImageUrl { set; get; }
        public float Price { set; get; }
        public float PrevPrice { set; get; }
        public int MarketId { get; set; }
        public string MarketName { set; get; }
        public string MarketImgUrl { get; set; }
        public bool IsFavorite { get; set; }


        public ProductVM(int id, string name, string imgUrl, float price, 
            float prevPrice, int marketId, string marketName, string marketUrl)
        {
            Id = id;
            Name = name;
            ImageUrl = imgUrl;
            Price = price;
            PrevPrice = prevPrice;
            MarketId = marketId;
            MarketName = marketName;
            MarketImgUrl = MarketImgUrl;
        }
    }
}