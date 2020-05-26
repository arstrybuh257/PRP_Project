using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GainBargain.DAL.Entities
{
    /// <summary>
    /// Represents a record about system's
    /// the most important activities.
    /// </summary>
    [Table("DbLogs")]
    public class DbLog
    {
        /// <summary>
        /// Id of the log.
        /// Just another junk field for PK.
        /// </summary>
        [Required]
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// When this log has been written.
        /// </summary>
        [Required]
        public DateTime Time{ get; set; }

        /// <summary>
        /// Code of the occured event.
        /// </summary>
        [Required]
        public LogCode Code { get; set; }

        /// <summary>
        /// Detailed description of the event.
        /// </summary>
        public string Info { get; set; }
        
        /// <summary>
        /// Code of the event which invoked logging.
        /// </summary>
        public enum LogCode
        {
            /// <summary>
            /// Not important message, used for debugging.
            /// </summary>
            Info = 0,

            /// <summary>
            /// Important error occured in the system.
            /// </summary>
            Error = 1
        }
    }
}
