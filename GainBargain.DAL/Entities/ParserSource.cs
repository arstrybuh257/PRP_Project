﻿namespace GainBargain.DAL.Entities
{
    public class ParserSource
    {
        /// <summary>
        /// Id of the entry
        /// </summary>
        public int Id { set; get; }

        /// <summary>
        /// Whether HTML or Json.
        /// 0 - HTML parser.
        /// 1 - Json parser.
        /// </summary>
        public byte ParserId { set; get; }

        /// <summary>
        /// Where to parse
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// Id of the shop (to take selectors from)
        /// </summary>
        public int MarketId { set; get; }

        /// <summary>
        /// Id of the category.
        /// </summary>
        public int CategoryId { set; get; }

        public virtual Market Market { set; get; }

        public virtual Category Category { set; get; }
    }
}
