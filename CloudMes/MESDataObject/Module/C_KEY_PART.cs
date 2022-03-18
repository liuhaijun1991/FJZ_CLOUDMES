using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;

namespace MESDataObject.Module
{
    public class T_C_KEY_PART : DataObjectTable
    {
        public T_C_KEY_PART(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_C_KEY_PART(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_C_KEY_PART);
            TableName = "C_KEY_PART".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }

        public List<C_KEY_PART> GetKeyPartList(OleExec db, string kpListID)
        {
            string sql = null;
            List<C_KEY_PART> lists = null;
            Row_C_KEY_PART rckp = null;
            DataTable dt = null;
            if (!string.IsNullOrEmpty(kpListID))
            {
                sql = $@" select * from {this.TableName} where keypart_list_id = '{kpListID}' order by seq_no asc ";
                dt = db.ExecSelect(sql).Tables[0];
                if (dt.Rows.Count > 0)
                {
                    lists = new List<C_KEY_PART>();
                    foreach (DataRow dr in dt.Rows)
                    {
                        rckp = (Row_C_KEY_PART) this.NewRow();
                        rckp.loadData(dr);
                        lists.Add(rckp.GetDataObject());
                    }
                }
                
            }

            return lists;
        }
    }
    public class Row_C_KEY_PART : DataObjectBase
    {
        public Row_C_KEY_PART(DataObjectInfo info) : base(info)
        {

        }
        public C_KEY_PART GetDataObject()
        {
            C_KEY_PART DataObject = new C_KEY_PART();
            DataObject.ID = this.ID;
            DataObject.KEYPART_LIST_ID = this.KEYPART_LIST_ID;
            DataObject.KEYPARTNO = this.KEYPARTNO;
            DataObject.DONOR_NAME = this.DONOR_NAME;
            DataObject.KEYPART_QTY = this.KEYPART_QTY;
            DataObject.KEYPART_TYPE = this.KEYPART_TYPE;
            DataObject.KEYPART_RULE = this.KEYPART_RULE;
            DataObject.STATION_NAME = this.STATION_NAME;
            DataObject.SEQ_NO = this.SEQ_NO;
            DataObject.LOCATION = this.LOCATION;
            DataObject.REPLACE_KEYPARTNO_FLAG = this.REPLACE_KEYPARTNO_FLAG;
            DataObject.MAIN_SKUNO = this.MAIN_SKUNO;
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
        public string KEYPART_LIST_ID
        {
            get
            {
                return (string)this["KEYPART_LIST_ID"];
            }
            set
            {
                this["KEYPART_LIST_ID"] = value;
            }
        }
        public string KEYPARTNO
        {
            get
            {
                return (string)this["KEYPARTNO"];
            }
            set
            {
                this["KEYPARTNO"] = value;
            }
        }
        public string DONOR_NAME
        {
            get
            {
                return (string)this["DONOR_NAME"];
            }
            set
            {
                this["DONOR_NAME"] = value;
            }
        }
        public double? KEYPART_QTY
        {
            get
            {
                return (double?)this["KEYPART_QTY"];
            }
            set
            {
                this["KEYPART_QTY"] = value;
            }
        }
        public string KEYPART_TYPE
        {
            get
            {
                return (string)this["KEYPART_TYPE"];
            }
            set
            {
                this["KEYPART_TYPE"] = value;
            }
        }
        public string KEYPART_RULE
        {
            get
            {
                return (string)this["KEYPART_RULE"];
            }
            set
            {
                this["KEYPART_RULE"] = value;
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
        public double? SEQ_NO
        {
            get
            {
                return (double?)this["SEQ_NO"];
            }
            set
            {
                this["SEQ_NO"] = value;
            }
        }
        public string LOCATION
        {
            get
            {
                return (string)this["LOCATION"];
            }
            set
            {
                this["LOCATION"] = value;
            }
        }
        public string REPLACE_KEYPARTNO_FLAG
        {
            get
            {
                return (string)this["REPLACE_KEYPARTNO_FLAG"];
            }
            set
            {
                this["REPLACE_KEYPARTNO_FLAG"] = value;
            }
        }
        public string MAIN_SKUNO
        {
            get
            {
                return (string)this["MAIN_SKUNO"];
            }
            set
            {
                this["MAIN_SKUNO"] = value;
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
    public class C_KEY_PART
    {
        public string ID{get;set;}
        public string KEYPART_LIST_ID{get;set;}
        public string KEYPARTNO{get;set;}
        public string DONOR_NAME{get;set;}
        public double? KEYPART_QTY{get;set;}
        public string KEYPART_TYPE{get;set;}
        public string KEYPART_RULE{get;set;}
        public string STATION_NAME{get;set;}
        public double? SEQ_NO{get;set;}
        public string LOCATION{get;set;}
        public string REPLACE_KEYPARTNO_FLAG{get;set;}
        public string MAIN_SKUNO{get;set;}
        public string EDIT_EMP{get;set;}
        public DateTime? EDIT_TIME{get;set;}
    }
}