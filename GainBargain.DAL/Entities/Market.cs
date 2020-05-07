﻿using System.ComponentModel.DataAnnotations;

namespace GainBargain.DAL.Entities
{
    public class Market
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }

        public string SelPrice { set; get; }
        public string SelName { set; get; }
        public string SelImageUrl { set; get; }
    }
}
