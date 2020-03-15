using System;
using System.Collections.Generic;
using GainBargain.DAL.Interfaces;

namespace GainBargain.Parser.Parsers
{
    /// <summary>
    /// Determines that the given class is able to retrieve
    /// product's information from its source
    /// </summary>
    /// <typeparam name="T">Value-type parameter (usually number) for price.</typeparam>
    public interface IProductParser<T>
        where T : struct,
                  IComparable,
                  IComparable<T>,
                  IConvertible,
                  IEquatable<T>,
                  IFormattable
    {
        IEnumerable<IParserOutput<T>> ParseInformation(IParserInput<T> input);
    }
}

