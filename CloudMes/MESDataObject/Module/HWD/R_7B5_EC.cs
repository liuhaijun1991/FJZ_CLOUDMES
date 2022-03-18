using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;

namespace MESDataObject.Module.HWD
{
    public class T_R_7B5_EC : DataObjectTable
    {
        public T_R_7B5_EC(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_R_7B5_EC(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_R_7B5_EC);
            TableName = "R_7B5_EC".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }

        public bool TaskNOExist(OleExec sfcdb, string task_no)
        {
            return sfcdb.ORM.Queryable<R_7B5_EC>().Any(r => r.TASK_NO == task_no);
        }

        public R_7B5_EC GetECObjectByTask(OleExec sfcdb, string task_no)
        {
            return sfcdb.ORM.Queryable<R_7B5_EC>().Where(r => r.TASK_NO == task_no).ToList().FirstOrDefault();
        }
        public DataTable GetECTable(OleExec sfcdb)
        {
            string sql = $@"select ID,STATUS,to_char(RECEIVE_DT,'YYYY/MM/DD') AS RECEIVE_DT,CUSTOMER_FILE_NAME,EC_CODE,to_char(FACTORY_EFFECT_DT,'YYYY/MM/DD') AS FACTORY_EFFECT_DT,
                            MODEL,ITEM,PARENT_ITEM,EC_CONTENT,ITEM_TYPE,EC_ITEM,STOCK_DEAL_REMARK,LINE,to_char(ONLINE_DT,'YYYY/MM/DD') AS ONLINE_DT,WO,
                            TASK_NO,OTHER,EC_TYPE,REMARK,EDIT_EMP,EDIT_DT from r_7b5_ec WHERE ID not like 'SYSTEM%'";
            return sfcdb.ExecuteDataTable(sql, CommandType.Text, null);
        }

        public List<R_7B5_EC> GetECList(OleExec sfcdb)
        {
            return sfcdb.ORM.Queryable<R_7B5_EC>().Where(r => !SqlSugar.SqlFunc.StartsWith(r.ID, "SYSTEM")).ToList();
        }
        public List<R_7B5_EC> GetListByCustomerFileName(OleExec sfcdb, string customerFileName)
        {
            return sfcdb.ORM.Queryable<R_7B5_EC>().Where(r => r.CUSTOMER_FILE_NAME == customerFileName).ToList();
        }

        public int DeleteRecordingByID(OleExec sfcdb,string ID)
        {
            return sfcdb.ORM.Deleteable<R_7B5_EC>().Where(r => r.ID == ID).ExecuteCommand();
        }

        public int SaveECObject(OleExec sfcdb, R_7B5_EC objEC)
        {
            return sfcdb.ORM.Insertable<R_7B5_EC>(objEC).ExecuteCommand();
        }

        public int UpdateECObject(OleExec sfcdb, R_7B5_EC objEC)
        {
            return sfcdb.ORM.Updateable<R_7B5_EC>().UpdateColumns(r => new R_7B5_EC
            {
            }).Where(r => r.ID == objEC.ID).ExecuteCommand();
        }
    }
    public class Row_R_7B5_EC : DataObjectBase
    {
        public Row_R_7B5_EC(DataObjectInfo info) : base(info)
        {

        }
        public R_7B5_EC GetDataObject()
        {
            R_7B5_EC DataObject = new R_7B5_EC();
            DataObject.REMARK = this.REMARK;
            DataObject.EC_TYPE = this.EC_TYPE;
            DataObject.OTHER = this.OTHER;
            DataObject.TASK_NO = this.TASK_NO;
            DataObject.WO = this.WO;
            DataObject.ONLINE_DT = this.ONLINE_DT;
            DataObject.LINE = this.LINE;
            DataObject.STOCK_DEAL_REMARK = this.STOCK_DEAL_REMARK;
            DataObject.EC_ITEM = this.EC_ITEM;
            DataObject.ITEM_TYPE = this.ITEM_TYPE;
            DataObject.EC_CONTENT = this.EC_CONTENT;
            DataObject.PARENT_ITEM = this.PARENT_ITEM;
            DataObject.ITEM = this.ITEM;
            DataObject.MODEL = this.MODEL;
            DataObject.FACTORY_EFFECT_DT = this.FACTORY_EFFECT_DT;
            DataObject.EC_CODE = this.EC_CODE;
            DataObject.CUSTOMER_FILE_NAME = this.CUSTOMER_FILE_NAME;
            DataObject.RECEIVE_DT = this.RECEIVE_DT;
            DataObject.STATUS = this.STATUS;
            DataObject.ID = this.ID;
            return DataObject;
        }
        public string REMARK
        {
            get
            {
                return (string)this["REMARK"];
            }
            set
            {
                this["REMARK"] = value;
            }
        }
        public string EC_TYPE
        {
            get
            {
                return (string)this["EC_TYPE"];
            }
            set
            {
                this["EC_TYPE"] = value;
            }
        }
        public string OTHER
        {
            get
            {
                return (string)this["OTHER"];
            }
            set
            {
                this["OTHER"] = value;
            }
        }
        public string TASK_NO
        {
            get
            {
                return (string)this["TASK_NO"];
            }
            set
            {
                this["TASK_NO"] = value;
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
        public DateTime? ONLINE_DT
        {
            get
            {
                return (DateTime?)this["ONLINE_DT"];
            }
            set
            {
                this["ONLINE_DT"] = value;
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
        public string STOCK_DEAL_REMARK
        {
            get
            {
                return (string)this["STOCK_DEAL_REMARK"];
            }
            set
            {
                this["STOCK_DEAL_REMARK"] = value;
            }
        }
        public string EC_ITEM
        {
            get
            {
                return (string)this["EC_ITEM"];
            }
            set
            {
                this["EC_ITEM"] = value;
            }
        }
        public string ITEM_TYPE
        {
            get
            {
                return (string)this["ITEM_TYPE"];
            }
            set
            {
                this["ITEM_TYPE"] = value;
            }
        }
        public string EC_CONTENT
        {
            get
            {
                return (string)this["EC_CONTENT"];
            }
            set
            {
                this["EC_CONTENT"] = value;
            }
        }
        public string PARENT_ITEM
        {
            get
            {
                return (string)this["PARENT_ITEM"];
            }
            set
            {
                this["PARENT_ITEM"] = value;
            }
        }
        public string ITEM
        {
            get
            {
                return (string)this["ITEM"];
            }
            set
            {
                this["ITEM"] = value;
            }
        }
        public string MODEL
        {
            get
            {
                return (string)this["MODEL"];
            }
            set
            {
                this["MODEL"] = value;
            }
        }
        public DateTime? FACTORY_EFFECT_DT
        {
            get
            {
                return (DateTime?)this["FACTORY_EFFECT_DT"];
            }
            set
            {
                this["FACTORY_EFFECT_DT"] = value;
            }
        }
        public string EC_CODE
        {
            get
            {
                return (string)this["EC_CODE"];
            }
            set
            {
                this["EC_CODE"] = value;
            }
        }
        public string CUSTOMER_FILE_NAME
        {
            get
            {
                return (string)this["CUSTOMER_FILE_NAME"];
            }
            set
            {
                this["CUSTOMER_FILE_NAME"] = value;
            }
        }
        public DateTime? RECEIVE_DT
        {
            get
            {
                return (DateTime?)this["RECEIVE_DT"];
            }
            set
            {
                this["RECEIVE_DT"] = value;
            }
        }
        public DateTime? EDIT_DT
        {
            get
            {
                return (DateTime?)this["EDIT_DT"];
            }
            set
            {
                this["EDIT_DT"] = value;
            }
        }
        public string STATUS
        {
            get
            {
                return (string)this["STATUS"];
            }
            set
            {
                this["STATUS"] = value;
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
    }
    public class R_7B5_EC
    {
        public string EDIT_EMP { get; set; }
        public DateTime? EDIT_DT { get; set; }
        public string REMARK { get; set; }
        public string EC_TYPE { get; set; }
        public string OTHER { get; set; }
        public string TASK_NO { get; set; }
        public string WO { get; set; }
        public DateTime? ONLINE_DT { get; set; }
        public string LINE { get; set; }
        public string STOCK_DEAL_REMARK { get; set; }
        public string EC_ITEM { get; set; }
        public string ITEM_TYPE { get; set; }
        public string EC_CONTENT { get; set; }
        public string PARENT_ITEM { get; set; }
        public string ITEM { get; set; }
        public string MODEL { get; set; }
        public DateTime? FACTORY_EFFECT_DT { get; set; }
        public string EC_CODE { get; set; }
        public string CUSTOMER_FILE_NAME { get; set; }
        public DateTime? RECEIVE_DT { get; set; }
        public string STATUS { get; set; }
        public string ID { get; set; }
    }
}