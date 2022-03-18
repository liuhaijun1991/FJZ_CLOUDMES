using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MESDataObject.Module
{

    // class R_AOI_FILEINFO
    //{
    /// <summary>
    /// FileInfo類,用於讀取aoi自動過站文件,生成FileInfo對象
    /// </summary>
    public class AOIFileInfo
    {   
        //線別
        private string _ProductLine;
        public string ProductLine
        {
            set { _ProductLine = value; }
            get { return _ProductLine; }
        }
        //測試日期
        private string _TestDate;
        public string TestDate
        {
            set { _TestDate = value; }
            get { return _TestDate; }
        }
        //文件名中帶出的SN
        private string _FileSN;
        public string FileSN
        {
            set { _FileSN = value; }
            get { return _FileSN; }
        }
        //連板狀態
        private string _LinkType;
        public string LinkType
        {
            set { _LinkType = value; }
            get { return _LinkType; }
        }
        //測試名稱
        private string _TestName;
        public string TestName
        {
            set { _TestName = value; }
            get { return _TestName; }
        }
        //測試工站
        private string _Station;
        public string Station
        {
            set { _Station = value; }
            get { return _Station; }
        }
        //File中的SN
       // private List<SNStatus> _FilesSNs;
        //public List<SNStatus> FilesSNs
        //{
        //    set { FilesSNs = value; }
        //    get { return FilesSNs; }
        //}
        //Link SN
        //private List<SNStatus> _LinkSNs;
        //public List<SNStatus> LinkSNs
        //{
        //    set { LinkSNs = value; }
        //    get { return LinkSNs; }
        //}
        public List<SNStatus> FilesSNs { get; set; }
        public List<SNStatus> LinkSNs { get; set; }

    }

    public class SNStatus
        {   
            //SN
            private string _SN;
            public string SN
            {
                set { _SN = value; }
                get { return _SN; }
            }
            //測試結果P or F
            private bool _Status;
            public bool Status
        {
                set { _Status = value; }
                get { return _Status; }
            }
            //錯誤總量
            private int _FailCount;
            public int FailCount
            {
                set { _FailCount = value; }
                get { return _FailCount; }
            }
            //錯誤詳情
            private string _FailDetail;
            public string FailDetail
            {
                set { _FailDetail = value; }
                get { return _FailDetail; }
            }
        }
    //}

   

    //private string regFileSNData = @"\w+_\d+;[P,F](;\d;(\w+,\w+;)+)?";
 



}
