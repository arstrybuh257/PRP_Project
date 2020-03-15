using GainBargain.DAL.Interfaces;

namespace GainBargain.DAL.Entities
{
    public class ParserSource : IParserInput<float>
    {
        public byte Id { set; get; }
        public string Url { get; set; }
        public int ShopId { set; get; }
        public string SelPrice { set; get; }
        public string SelName { set; get; }
        public string SelImageUrl { set; get; }
    }
}
