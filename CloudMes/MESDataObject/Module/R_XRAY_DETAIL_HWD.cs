using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;

namespace MESDataObject.Module
{
    public class T_R_XRAY_DETAIL_HWD : DataObjectTable
    {
        public T_R_XRAY_DETAIL_HWD(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_R_XRAY_DETAIL_HWD(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_R_XRAY_DETAIL_HWD);
            TableName = "R_XRAY_DETAIL_HWD".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }
    }
    public class Row_R_XRAY_DETAIL_HWD : DataObjectBase
    {
        public Row_R_XRAY_DETAIL_HWD(DataObjectInfo info) : base(info)
        {

        }
        public R_XRAY_DETAIL_HWD GetDataObject()
        {
            R_XRAY_DETAIL_HWD DataObject = new R_XRAY_DETAIL_HWD();
            DataObject.EDIT_EMP = this.EDIT_EMP;
            DataObject.EDIT_TIME = this.EDIT_TIME;
            DataObject.SNID = this.SNID;
            DataObject.XRAYID = this.XRAYID;
            DataObject.ID = this.ID;
            DataObject.SKUNO = this.SKUNO;
            DataObject.LINE = this.LINE;
            DataObject.STATION = this.STATION;
            DataObject.WO = this.WO;
            return DataObject;
        }
        public string EDIT_EMP
        {
            get
            {
                return (string)this["EDIT_EMP"];
            }
            set
            {
                this["EDIT_EMP"] = value;
            }
        }
        public DateTime? EDIT_TIME
        {
            get
            {
                return (DateTime?)this["EDIT_TIME"];
            }
            set
            {
                this["EDIT_TIME"] = value;
            }
        }
        public string SNID
        {
            get
            {
                return (string)this["SNID"];
            }
            set
            {
                this["SNID"] = value;
            }
        }
        public string XRAYID
        {
            get
            {
                return (string)this["XRAYID"];
            }
            set
            {
                this["XRAYID"] = value;
            }
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
        public string SKUNO
        {
            get
            {
                return (string)this["SKUNO"];
            }
            set
            {
                this["SKUNO"] = value;
            }
        }
        public string LINE
        {
            get
            {
                return (string)this["LINE"];
            }
            set
            {
                this["LINE"] = value;
            }
        }
        public string STATION
        {
            get
            {
                return (string)this["STATION"];
            }
            set
            {
                this["STATION"] = value;
            }
        }
        public string WO
        {
            get
            {
                return (string)this["WO"];
            }
            set
            {
                this["WO"] = value;
            }
        }
    }
    public class R_XRAY_DETAIL_HWD
    {
        public string EDIT_EMP { get; set; }
        public DateTime? EDIT_TIME { get; set; }
        public string SNID { get; set; }
        public string XRAYID { get; set; }
        public string ID { get; set; }
        public string SKUNO { get; set; }
        public string LINE { get; set; }
        public string STATION { get; set; }
        public string WO { get; set; }
    }
}