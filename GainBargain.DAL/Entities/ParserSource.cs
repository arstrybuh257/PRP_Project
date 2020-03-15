using GainBargain.DAL.Interfaces;

namespace GainBargain.DAL.Entities
{
    public class ParserSource : IParserInput<float>
    {
        public int Id { set; get; }
        public byte ParserId { set; get; }
        public string Url { get; set; }

        public int MarketId { set; get; }
        public int CategoryId { set; get; }

        public string SelPrice { set; get; }
        public string SelName { set; get; }
        public string SelImageUrl { set; get; }

        public virtual Market Market { set; get; }
    }
}
