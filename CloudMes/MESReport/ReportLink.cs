using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MESReport
{
    public class ReportLink
    {
        /// <summary>
        /// MESReport,URL
        /// </summary>
        public string LinkType = "MESReport";
        public string URL = "";
        public string ClassName = "";
        /// <summary>
        /// 字典里存放鏈接報表的參數
        /// </summary>
        public Dictionary<string, string> Data = new Dictionary<string, string>();
    }
}
