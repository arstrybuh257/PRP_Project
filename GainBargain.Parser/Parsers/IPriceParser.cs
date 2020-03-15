using System;
using System.Collections.Generic;

namespace SalesParser.Parsers
{
    /// <summary>
    /// Determines that the given class is able to retrieve
    /// prices from its source
    /// </summary>
    /// <typeparam name="T">Value-type parameter (usually number) for price.</typeparam>
    public interface IPriceParser<T>
        where T : struct,
                  IComparable,
                  IComparable<T>,
                  IConvertible,
                  IEquatable<T>,
                  IFormattable
    {
        IEnumerable<T> GetAllPrices(string retrievalParam);
    }
}

