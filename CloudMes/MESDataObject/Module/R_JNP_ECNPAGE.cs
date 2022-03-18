using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;

namespace MESDataObject.Module
{
    public class T_R_JNP_ECNPAGE : DataObjectTable
    {
        public T_R_JNP_ECNPAGE(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_R_JNP_ECNPAGE(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_R_JNP_ECNPAGE);
            TableName = "R_JNP_ECNPAGE".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }
    }
    public class Row_R_JNP_ECNPAGE : DataObjectBase
    {
        public Row_R_JNP_ECNPAGE(DataObjectInfo info) : base(info)
        {

        }
        public R_JNP_ECNPAGE GetDataObject()
        {
            R_JNP_ECNPAGE DataObject = new R_JNP_ECNPAGE();
            DataObject.ID = this.ID;
            DataObject.CHANGENO = this.CHANGENO;
            DataObject.CUSTCHANGENO = this.CUSTCHANGENO;
            DataObject.ITEMNUMBER = this.ITEMNUMBER;
            DataObject.REV = this.REV;
            DataObject.ECOREPORTING = this.ECOREPORTING;
            DataObject.CUSTREQUIRDATE = this.CUSTREQUIRDATE;
            DataObject.ACTIVED = this.ACTIVED;
            DataObject.CREATEBY = this.CREATEBY;
            DataObject.CREATEDATE = this.CREATEDATE;
            DataObject.PLANT = this.PLANT;
            DataObject.MSGID = this.MSGID;
            return DataObject;
        }
        public string ID
        {
            get
            {
                return (string)this["ID"];
            }
            set
            {
                this["ID"] = value;
            }
        }
        public string CHANGENO
        {
            get
            {
                return (string)this["CHANGENO"];
            }
            set
            {
                this["CHANGENO"] = value;
            }
        }
        public string CUSTCHANGENO
        {
            get
            {
                return (string)this["CUSTCHANGENO"];
            }
            set
            {
                this["CUSTCHANGENO"] = value;
            }
        }
        public string ITEMNUMBER
        {
            get
            {
                return (string)this["ITEMNUMBER"];
            }
            set
            {
                this["ITEMNUMBER"] = value;
            }
        }
        public string REV
        {
            get
            {
                return (string)this["REV"];
            }
            set
            {
                this["REV"] = value;
            }
        }
        public string ECOREPORTING
        {
            get
            {
                return (string)this["ECOREPORTING"];
            }
            set
            {
                this["ECOREPORTING"] = value;
            }
        }
        public string CUSTREQUIRDATE
        {
            get
            {
                return (string)this["CUSTREQUIRDATE"];
            }
            set
            {
                this["CUSTREQUIRDATE"] = value;
            }
        }
        public string ACTIVED
        {
            get
            {
                return (string)this["ACTIVED"];
            }
            set
            {
                this["ACTIVED"] = value;
            }
        }
        public string CREATEBY
        {
            get
            {
                return (string)this["CREATEBY"];
            }
            set
            {
                this["CREATEBY"] = value;
            }
        }
        public DateTime? CREATEDATE
        {
            get
            {
                return (DateTime?)this["CREATEDATE"];
            }
            set
            {
                this["CREATEDATE"] = value;
            }
        }
        public string PLANT
        {
            get
            {
                return (string)this["PLANT"];
            }
            set
            {
                this["PLANT"] = value;
            }
        }
        public string MSGID
        {
            get
            {
                return (string)this["MSGID"];
            }
            set
            {
                this["MSGID"] = value;
            }
        }
    }
    public class R_JNP_ECNPAGE
    {
        /// <summary>
        /// Mes系统自动生成,无需填充
        /// </summary>
        [SqlSugar.SugarColumn(IsPrimaryKey = true)]
        public string ID { get; set; }
        /// <summary>
        /// Fox Change Number
        /// </summary>
        public string CHANGENO { get; set; }
        /// <summary>
        /// ECN.Page Two.Customer Change Number
        /// </summary>
        public string CUSTCHANGENO { get; set; }
        /// <summary>
        /// EECN.Affected Item.ItemNumber
        /// </summary>
        public string ITEMNUMBER { get; set; }
        /// <summary>
        /// ECN.Affected Item.New Rev
        /// </summary>
        public string REV { get; set; }
        /// <summary>
        /// ECN.Page Three.ECO Reporting
        /// </summary>
        public string ECOREPORTING { get; set; }
        /// <summary>
        /// ECN.Page Three.Customer Required Implementation Date
        /// </summary>
        public string CUSTREQUIRDATE { get; set; }
        /// <summary>
        /// Mes系统自动生成,无需填充
        /// </summary>
        public string ACTIVED { get; set; }
        /// <summary>
        /// Mes系统自动生成,无需填充
        /// </summary>
        public string CREATEBY { get; set; }
        /// <summary>
        /// Mes系统自动生成,无需填充
        /// </summary>
        public DateTime? CREATEDATE { get; set; }
        /// <summary>
        /// Mes系统自动生成,无需填充
        /// </summary>
        public string PLANT { get; set; }
        /// <summary>
        /// Mes系统自动生成,无需填充
        /// </summary>
        public string MSGID { get; set; }
    }
}