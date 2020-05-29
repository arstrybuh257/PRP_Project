using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GainBargain.WEB.Models
{
    public class ProductVM
    {
        public string Name { get; set; }
        public string ImageUrl { set; get; }
        public float Price { set; get; }
        public float PrevPrice { set; get; }
        public int MarketId { get; set; }
        public string MarketName { set; get; }
        public string MarketImgUrl { get; set; }

        public ProductVM(string name, string imgUrl, float price, 
            float prevPrice, int marketId, string marketName, string marketUrl)
        {
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