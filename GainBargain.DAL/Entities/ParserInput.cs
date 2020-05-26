using System.ComponentModel.DataAnnotations.Schema;

namespace GainBargain.DAL.Entities
{
    /// <summary>
    /// This class is used as input for the parser.
    /// </summary>
    [NotMapped]
    public class ParserInput : ParserSource
    {
        /// <summary>
        /// How to get product's price
        /// </summary>
        public string SelPrice { set; get; }

        /// <summary>
        /// How to get product's description
        /// </summary>
        public string SelName { set; get; }

        /// <summary>
        /// How to get product's image url
        /// </summary>
        public string SelImageUrl { set; get; }

        public ParserInput() { }

        public ParserInput(ParserSource source, Market market)
        {
            this.ParserId = source.ParserId;
            this.Url = source.Url;
            this.MarketId = source.MarketId;
            this.CategoryId = source.CategoryId;

            this.SelPrice = market.SelPrice;
            this.SelName = market.SelName;
            this.SelImageUrl = market.SelImageUrl;
        }
    }
}
