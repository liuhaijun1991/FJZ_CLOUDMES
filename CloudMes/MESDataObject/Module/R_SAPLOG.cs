using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;

namespace MESDataObject.Module
{
    public class T_R_SAPLOG : DataObjectTable
    {
        public T_R_SAPLOG(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_R_SAPLOG(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_R_SAPLOG);
            TableName = "R_SAPLOG".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }
    }
    public class Row_R_SAPLOG : DataObjectBase
    {
        public Row_R_SAPLOG(DataObjectInfo info) : base(info)
        {

        }
        public R_SAPLOG GetDataObject()
        {
            R_SAPLOG DataObject = new R_SAPLOG();
            DataObject.ID = this.ID;
            DataObject.WORKTIME = this.WORKTIME;
            DataObject.ERRORMESSAGE = this.ERRORMESSAGE;
            DataObject.QTY = this.QTY;
            DataObject.SKUNO = this.SKUNO;
            DataObject.ERRORTYPE = this.ERRORTYPE;
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
        public DateTime? WORKTIME
        {
            get
            {
                return (DateTime?)this["WORKTIME"];
            }
            set
            {
                this["WORKTIME"] = value;
            }
        }
        public string ERRORMESSAGE
        {
            get
            {
                return (string)this["ERRORMESSAGE"];
            }
            set
            {
                this["ERRORMESSAGE"] = value;
            }
        }
        public double? QTY
        {
            get
            {
                return (double?)this["QTY"];
            }
            set
            {
                this["QTY"] = value;
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
        public string ERRORTYPE
        {
            get
            {
                return (string)this["ERRORTYPE"];
            }
            set
            {
                this["ERRORTYPE"] = value;
            }
        }
    }
    public class R_SAPLOG
    {
        public string ID{get;set;}
        public DateTime? WORKTIME{get;set;}
        public string ERRORMESSAGE{get;set;}
        public double? QTY{get;set;}
        public string SKUNO{get;set;}
        public string ERRORTYPE{get;set;}
    }
}