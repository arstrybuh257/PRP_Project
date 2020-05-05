using System;

namespace GainBargain.Parser.Interfaces
{
    public interface IParserInput<T>
        where T : struct,
                  IComparable,
                  IComparable<T>,
                  IConvertible,
                  IEquatable<T>,
                  IFormattable
    {
        string Url { set; get; }
        byte ParserId { set; get; }

        int MarketId { set; get; }
        int CategoryId { set; get; }

        string SelPrice { set; get; }
        string SelName { set; get; }
        string SelImageUrl { set; get; }
    }
}
