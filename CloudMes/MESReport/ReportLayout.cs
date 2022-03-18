using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MESReport
{
    /// <summary>
    /// Layout Container
    /// </summary>
    public class ReportLayout
    {
        /// <summary>
        /// Container ID
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// Scale is the amount of column occupied in the Bootstrap flexbox grid system
        /// </summary>
        public int Scale { get; set; }
    }
}
