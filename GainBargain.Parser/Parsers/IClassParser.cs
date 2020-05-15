using System;
using System.Collections.Generic;

namespace GainBargain.Parser.Parsers
{
    public interface IClassParser<Input, Output>
        where Input : new() 
        where Output : new()
    {
        /// <summary>
        /// Basically, takes input, parses given source and post processes
        /// the result before returning it.
        /// </summary>
        IEnumerable<Output> Parse(Input input);
    }
}

