using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;

namespace MESDataObject.Module.HWT
{

    public class T_R_SKUNO_ADDRESS : DataObjectTable
    {
        public T_R_SKUNO_ADDRESS(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_R_SKUNO_ADDRESS(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_R_SKUNO_ADDRESS);
            TableName = "R_SKUNO_ADDRESS".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }

        /// <summary>
        /// add by hgb 2019.08.16
        /// </summary>
        /// <param name="StrSN"></param>
        /// <param name="DB"></param>
        /// <returns></returns>
        public R_SKUNO_ADDRESS LoadData(string SKUNO,string DATA2, OleExec DB)
        {
            if (DATA2=="")
            {
                return DB.ORM.Queryable<R_SKUNO_ADDRESS>().Where(t => t.FSKUNO == SKUNO).ToList().FirstOrDefault();
            }
            else
            {
                return DB.ORM.Queryable<R_SKUNO_ADDRESS>().Where(t => t.FSKUNO == SKUNO && t.DATA2== DATA2).ToList().FirstOrDefault();
            }
           
        }

        /// <summary>
        /// add by hgb 2019.08.16
        /// </summary>
        /// <param name="StrSN"></param>
        /// <param name="DB"></param>
        /// <returns></returns>
        public bool IsExists(string SKUNO, string DATA2, OleExec DB)
        {
            if (DATA2 == "")
            {
                return DB.ORM.Queryable<R_SKUNO_ADDRESS>().Any(t => t.FSKUNO == SKUNO);
            }
            else
            {
                return DB.ORM.Queryable<R_SKUNO_ADDRESS>().Any(t => t.FSKUNO == SKUNO && t.DATA2 == DATA2);
            }

        }
    }
    public class Row_R_SKUNO_ADDRESS : DataObjectBase
    {
        public Row_R_SKUNO_ADDRESS(DataObjectInfo info) : base(info)
        {

        }
        public R_SKUNO_ADDRESS GetDataObject()
        {
            R_SKUNO_ADDRESS DataObject = new R_SKUNO_ADDRESS();
            DataObject.ID = this.ID;
            DataObject.FSKUNO = this.FSKUNO;
            DataObject.HSKUNO = this.HSKUNO;
            DataObject.DELIVER_ADDRESS = this.DELIVER_ADDRESS;
            DataObject.SHIPPING_ADDRESS = this.SHIPPING_ADDRESS;
            DataObject.ORDER_TIME = this.ORDER_TIME;
            DataObject.DELAY_TIME = this.DELAY_TIME;
            DataObject.SHIPPING_TIME = this.SHIPPING_TIME;
            DataObject.DATA2 = this.DATA2;
            DataObject.SHIPTOCODE = this.SHIPTOCODE;
            DataObject.EDIT_TIME = this.EDIT_TIME;
            DataObject.EDIT_EMP = this.EDIT_EMP;
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
        public string FSKUNO
        {
            get
            {
                return (string)this["FSKUNO"];
            }
            set
            {
                this["FSKUNO"] = value;
            }
        }
        public string HSKUNO
        {
            get
            {
                return (string)this["HSKUNO"];
            }
            set
            {
                this["HSKUNO"] = value;
            }
        }
        public string DELIVER_ADDRESS
        {
            get
            {
                return (string)this["DELIVER_ADDRESS"];
            }
            set
            {
                this["DELIVER_ADDRESS"] = value;
            }
        }
        public string SHIPPING_ADDRESS
        {
            get
            {
                return (string)this["SHIPPING_ADDRESS"];
            }
            set
            {
                this["SHIPPING_ADDRESS"] = value;
            }
        }
        public DateTime? ORDER_TIME
        {
            get
            {
                return (DateTime?)this["ORDER_TIME"];
            }
            set
            {
                this["ORDER_TIME"] = value;
            }
        }
        public string DELAY_TIME
        {
            get
            {
                return (string)this["DELAY_TIME"];
            }
            set
            {
                this["DELAY_TIME"] = value;
            }
        }
        public DateTime? SHIPPING_TIME
        {
            get
            {
                return (DateTime?)this["SHIPPING_TIME"];
            }
            set
            {
                this["SHIPPING_TIME"] = value;
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
        public string SHIPTOCODE
        {
            get
            {
                return (string)this["SHIPTOCODE"];
            }
            set
            {
                this["SHIPTOCODE"] = value;
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
    public class R_SKUNO_ADDRESS
    {
        public string ID{get;set;}
        public string FSKUNO{get;set;}
        public string HSKUNO{get;set;}
        public string DELIVER_ADDRESS{get;set;}
        public string SHIPPING_ADDRESS{get;set;}
        public DateTime? ORDER_TIME{get;set;}
        public string DELAY_TIME{get;set;}
        public DateTime? SHIPPING_TIME{get;set;}
        public string DATA2{get;set;}
        public string SHIPTOCODE{get;set;}
        public DateTime? EDIT_TIME{get;set;}
        public string EDIT_EMP{get;set;}
    }
}
