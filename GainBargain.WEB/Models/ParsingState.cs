using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GainBargain.WEB.Models
{
    public class ParsingState
    {
        public bool IsParsing { private set; get; }

        public int SourcesParsed { private set; get; }

        public int SourcesTotal { private set; get; }

        public void ParsingStarted(int total)
        {
            IsParsing = true;
            SourcesParsed = 0;
            SourcesTotal = total;
        }

        public bool IncrementDoneSources()
        {
            return IsParsing = ++SourcesParsed != SourcesTotal;
        }

        public void ParsingFinished()
        {
            IsParsing = false;
        }
    }
}