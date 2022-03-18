using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;

namespace MESDataObject.Module.HWT
{
    public class T_R_FT_TBS : DataObjectTable
    {
        public T_R_FT_TBS(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_R_FT_TBS(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_R_FT_TBS);
            TableName = "R_FT_TBS".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }
    }
    public class Row_R_FT_TBS : DataObjectBase
    {
        public Row_R_FT_TBS(DataObjectInfo info) : base(info)
        {

        }
        public R_FT_TBS GetDataObject()
        {
            R_FT_TBS DataObject = new R_FT_TBS();
            DataObject.ID = this.ID;
            DataObject.MODEL_NAME = this.MODEL_NAME;
            DataObject.STATION_NAME = this.STATION_NAME;
            DataObject.TBS_NAME = this.TBS_NAME;
            DataObject.MEMO1 = this.MEMO1;
            DataObject.INPUT_TIME = this.INPUT_TIME;
            DataObject.INPUT_EMP = this.INPUT_EMP;
            DataObject.DATA1 = this.DATA1;
            DataObject.DATA2 = this.DATA2;
            DataObject.EDIT_EMP = this.EDIT_EMP;
            DataObject.EDIT_TIME = this.EDIT_TIME;
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
        public string MODEL_NAME
        {
            get
            {
                return (string)this["MODEL_NAME"];
            }
            set
            {
                this["MODEL_NAME"] = value;
            }
        }
        public string STATION_NAME
        {
            get
            {
                return (string)this["STATION_NAME"];
            }
            set
            {
                this["STATION_NAME"] = value;
            }
        }
        public string TBS_NAME
        {
            get
            {
                return (string)this["TBS_NAME"];
            }
            set
            {
                this["TBS_NAME"] = value;
            }
        }
        public string MEMO1
        {
            get
            {
                return (string)this["MEMO1"];
            }
            set
            {
                this["MEMO1"] = value;
            }
        }
        public DateTime? INPUT_TIME
        {
            get
            {
                return (DateTime?)this["INPUT_TIME"];
            }
            set
            {
                this["INPUT_TIME"] = value;
            }
        }
        public string INPUT_EMP
        {
            get
            {
                return (string)this["INPUT_EMP"];
            }
            set
            {
                this["INPUT_EMP"] = value;
            }
        }
        public string DATA1
        {
            get
            {
                return (string)this["DATA1"];
            }
            set
            {
                this["DATA1"] = value;
            }
        }
        public string DATA2
        {
            get
            {
                return (string)this["DATA2"];
            }
            set
            {
                this["DATA2"] = value;
            }
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
    }
    public class R_FT_TBS
    {
        public string ID{ get; set; }
        public string MODEL_NAME{ get; set; }
        public string STATION_NAME{ get; set; }
        public string TBS_NAME{ get; set; }
        public string MEMO1{ get; set; }
        public DateTime? INPUT_TIME{ get; set; }
        public string INPUT_EMP{ get; set; }
        public string DATA1{ get; set; }
        public string DATA2{ get; set; }
        public string EDIT_EMP{ get; set; }
        public DateTime? EDIT_TIME{ get; set; }
    }
}