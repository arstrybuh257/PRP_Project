using System;

namespace GainBargain.DAL.Interfaces
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
        byte Id { set; get; }
        int ShopId { set; get; }
        string SelPrice { set; get; }
        string SelName { set; get; }
        string SelImageUrl { set; get; }
    }
}
