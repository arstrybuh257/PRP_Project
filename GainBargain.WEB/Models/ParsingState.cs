using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace GainBargain.WEB.Models
{
    /// <summary>
    /// Holds current parsing state of the system.
    /// </summary>
    public class ParsingState
    {
        /// <summary>
        /// When the parsing has started.
        /// </summary>
        private DateTime startTime;

        /// <summary>
        /// Whether the parsing process is executing now.
        /// </summary>
        public bool IsParsing { private set; get; }

        /// <summary>
        /// When the parsing has started.
        /// </summary>
        public string StartTime { get => startTime.ToString("mm:HH dd.MM.YYYY"); }

        /// <summary>
        /// How many parsing sources has been parsed by now.
        /// </summary>
        public int SourcesParsed { private set; get; }

        /// <summary>
        /// How many parsing sources must be processed in this parsing cycle.
        /// </summary>
        public int SourcesTotal { private set; get; }

        /// <summary>
        /// Fase of the parsing.
        /// If not parsing: -1.
        /// If not all sources are processed: 0.
        /// If optimization stage: 1.
        /// </summary>
        public int Stage
        {
            get
            {
                if (!IsParsing)
                {
                    return -1;
                }
                if (SourcesParsed < SourcesTotal)
                {
                    return 0;
                }
                return 1;
            }
        }

        /// <summary>
        /// Tell this object that the parsing has started.
        /// </summary>
        /// <param name="total">How many parsing sources will be processed.</param>
        public void ParsingStarted(int total)
        {
            IsParsing = true;
            SourcesParsed = 0;
            SourcesTotal = total;

            startTime = DateTime.Now;
        }

        /// <summary>
        /// Tell this object that a data source has been processed.
        /// Note: this method can automatically turn its state to finished
        /// when all the parsing sources are processed.
        /// </summary>
        public void IncrementDoneSources()
        {
            if (++SourcesParsed > SourcesTotal)
            {
                throw new Exception("Parsed more than was planning!");
            }
        }

        /// <summary>
        /// Manually interrupt parsing process.
        /// </summary>
        public void ParsingFinished()
        {
            IsParsing = false;
        }
    }
}