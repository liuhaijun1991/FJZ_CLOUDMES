using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;

namespace MESDataObject.Module
{
    public class T_R_SAP_LOG : DataObjectTable
    {
        public T_R_SAP_LOG(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_R_SAP_LOG(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_R_SAP_LOG);
            TableName = "R_SAP_LOG".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }
    }
    public class Row_R_SAP_LOG : DataObjectBase
    {
        public Row_R_SAP_LOG(DataObjectInfo info) : base(info)
        {

        }
        public R_SAP_LOG GetDataObject()
        {
            R_SAP_LOG DataObject = new R_SAP_LOG();
            DataObject.ID = this.ID;
            DataObject.LOGKEY = this.LOGKEY;
            DataObject.CID = this.CID;
            DataObject.RES = this.RES;
            DataObject.INID = this.INID;
            DataObject.OUTID = this.OUTID;
            DataObject.CREATETIME = this.CREATETIME;
            DataObject.CREATEBY = this.CREATEBY;
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
        public string LOGKEY
        {
            get
            {
                return (string)this["LOGKEY"];
            }
            set
            {
                this["LOGKEY"] = value;
            }
        }
        public string CID
        {
            get
            {
                return (string)this["CID"];
            }
            set
            {
                this["CID"] = value;
            }
        }
        public string RES
        {
            get
            {
                return (string)this["RES"];
            }
            set
            {
                this["RES"] = value;
            }
        }
        public string INID
        {
            get
            {
                return (string)this["INID"];
            }
            set
            {
                this["INID"] = value;
            }
        }
        public string OUTID
        {
            get
            {
                return (string)this["OUTID"];
            }
            set
            {
                this["OUTID"] = value;
            }
        }
        public DateTime? CREATETIME
        {
            get
            {
                return (DateTime?)this["CREATETIME"];
            }
            set
            {
                this["CREATETIME"] = value;
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
    }
    public class R_SAP_LOG
    {
        [SqlSugar.SugarColumn(IsPrimaryKey =true)]
        public string ID { get; set; }
        public string LOGKEY { get; set; }
        public string CID { get; set; }
        public string RES { get; set; }
        public string INID { get; set; }
        public string OUTID { get; set; }
        public DateTime? CREATETIME { get; set; }
        public string CREATEBY { get; set; }
    }
}