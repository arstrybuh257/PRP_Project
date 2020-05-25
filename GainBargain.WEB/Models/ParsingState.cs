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
        /// Whether the parsing process is executing now.
        /// </summary>
        public bool IsParsing { private set; get; }

        /// <summary>
        /// When the parsing has started.
        /// </summary>
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime StartTime { private set; get; }

        /// <summary>
        /// How many parsing sources has been parsed by now.
        /// </summary>
        public int SourcesParsed { private set; get; }

        /// <summary>
        /// How many parsing sources must be processed in this parsing cycle.
        /// </summary>
        public int SourcesTotal { private set; get; }

        /// <summary>
        /// Tell this object that the parsing has started.
        /// </summary>
        /// <param name="total">How many parsing sources will be processed.</param>
        public void ParsingStarted(int total)
        {
            IsParsing = true;
            SourcesParsed = 0;
            SourcesTotal = total;

            StartTime = DateTime.Now;
        }

        /// <summary>
        /// Tell this object that a data source has been processed.
        /// Note: this method can automatically turn its state to finished
        /// when all the parsing sources are processed.
        /// </summary>
        /// <returns></returns>
        public bool IncrementDoneSources()
        {
            return IsParsing = ++SourcesParsed != SourcesTotal;
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