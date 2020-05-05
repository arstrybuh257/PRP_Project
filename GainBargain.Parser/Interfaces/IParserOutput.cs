using System;

namespace GainBargain.Parser.Interfaces
{
    public interface IParserOutput<T>
            where T : struct,
                      IComparable,
                      IComparable<T>,
                      IConvertible,
                      IEquatable<T>,
                      IFormattable
    {
        int MarketId { set; get; }
        int CategoryId { set; get; }
        string Name { set; get; }
        DateTime UploadTime { set; get; }

        T Price { set; get; }
        string ImageUrl { set; get; }
    }
}
