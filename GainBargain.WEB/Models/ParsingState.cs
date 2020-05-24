﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace GainBargain.WEB.Models
{
    public class ParsingState
    {
        public bool IsParsing { private set; get; }

        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime StartTime { private set; get; }

        public int SourcesParsed { private set; get; }

        public int SourcesTotal { private set; get; }

        public void ParsingStarted(int total)
        {
            IsParsing = true;
            SourcesParsed = 0;
            SourcesTotal = total;

            StartTime = DateTime.Now;
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