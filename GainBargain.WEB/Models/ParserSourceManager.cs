namespace GainBargain.WEB.Models
{
    public class ParserSourceManager
    {
        public int Id { set; get; }
        //public byte ParserId { set; get; }
        public int MarketId { set; get; }
        public string MarketName { set; get; }
        public int CategoryId { set; get; }
        public string CategoryName { set; get; }
        public string Url { get; set; }
    }
}