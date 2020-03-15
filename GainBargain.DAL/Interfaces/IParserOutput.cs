using System;

namespace GainBargain.DAL.Interfaces
{
    interface IParserOutput<T>
        where T : struct,
                  IComparable,
                  IComparable<T>,
                  IConvertible,
                  IEquatable<T>,
                  IFormattable
    {
        int MarketId { set; get; }
        string Name { set; get; }
        DateTime UploadTime { set; get; }

        T Price { set; get; }
        string ImageUrl { set; get; }
    }
}
