using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MESReport
{
    public class ReportOutputBase
    {
        /// <summary>
        /// 容器ID，ReportLayout的ID，如果沒有設置佈局可爲空
        /// </summary>
        public string ContainerID = null;
    }

    /// <summary>
    /// 提供Alart
    /// </summary>
    public class ReportAlart : ReportOutputBase
    {
        public string Msg { get; set; }
        public string AlartType = "warning";
        public String OutputType
        {
            get
            {
                return "ReportAlart";
            }
        }
        public ReportAlart(string _Msg) { Msg = _Msg; }
        public ReportAlart() { }
    }

    public class ReportFile : ReportOutputBase
    {
        public String OutputType
        {
            get
            {
                return "ReportFile";
            }
        }
        public string FileName { get; set; }
        public object FileContent { get; set; }
        public ReportFile(string file_name, object file_content)
        {
            FileName = file_name;
            FileContent = file_content;
        }
        public ReportFile() { }
    }

    public class ReportColumns : ReportOutputBase
    {
        public String OutputType
        {
            get
            {
                return "ReportColumns";
            }
        }
        public object Columns { get; set; }
        public ReportColumns(object _columns)
        {
            Columns = _columns;
        }
        public ReportColumns() { }
    }
}