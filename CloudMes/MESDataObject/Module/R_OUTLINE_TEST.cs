using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;

namespace MESDataObject.Module
{
    public class T_R_OUTLINE_TEST : DataObjectTable
    {
        public T_R_OUTLINE_TEST(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_R_OUTLINE_TEST(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_R_OUTLINE_TEST);
            TableName = "R_OUTLINE_TEST".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }
        public int Save(OleExec DB, R_OUTLINE_TEST obj)
        {
            return DB.ORM.Insertable<R_OUTLINE_TEST>(obj).ExecuteCommand();
        }
        public int Update(OleExec DB, R_OUTLINE_TEST obj)
        {
            return DB.ORM.Updateable<R_OUTLINE_TEST>(obj).Where(r => r.ID == obj.ID).ExecuteCommand();
        }

        public bool SNExist(OleExec DB, string type,string sn)
        {
            return DB.ORM.Queryable<R_OUTLINE_TEST>().Where(r => r.SN == sn && r.DATA_TYPE == type && r.VALID_FLAG == 1).Any();
        }

    }
    public class Row_R_OUTLINE_TEST : DataObjectBase
    {
        public Row_R_OUTLINE_TEST(DataObjectInfo info) : base(info)
        {

        }
        public R_OUTLINE_TEST GetDataObject()
        {
            R_OUTLINE_TEST DataObject = new R_OUTLINE_TEST();
            DataObject.DATA_TYPE = this.DATA_TYPE;
            DataObject.LASTEDIT_BY = this.LASTEDIT_BY;
            DataObject.LASTEDIT_DATE = this.LASTEDIT_DATE;
            DataObject.CREAT_EMP = this.CREAT_EMP;
            DataObject.CREAT_DATE = this.CREAT_DATE;
            DataObject.VALID_FLAG = this.VALID_FLAG;
            DataObject.FAIL_DESC = this.FAIL_DESC;
            DataObject.STATUS = this.STATUS;
            DataObject.LINE = this.LINE;
            DataObject.STATION = this.STATION;
            DataObject.SKUNO = this.SKUNO;
            DataObject.WORKORDERNO = this.WORKORDERNO;
            DataObject.SN = this.SN;
            DataObject.ID = this.ID;
            return DataObject;
        }
        
        public string DATA_TYPE
        {
            get
            {
                return (string)this["DATA_TYPE"];
            }
            set
            {
                this["DATA_TYPE"] = value;
            }
        }
        public string LASTEDIT_BY
        {
            get
            {
                return (string)this["LASTEDIT_BY"];
            }
            set
            {
                this["LASTEDIT_BY"] = value;
            }
        }
        public DateTime? LASTEDIT_DATE
        {
            get
            {
                return (DateTime?)this["LASTEDIT_DATE"];
            }
            set
            {
                this["LASTEDIT_DATE"] = value;
            }
        }
        public string CREAT_EMP
        {
            get
            {
                return (string)this["CREAT_EMP"];
            }
            set
            {
                this["CREAT_EMP"] = value;
            }
        }
        public DateTime? CREAT_DATE
        {
            get
            {
                return (DateTime?)this["CREAT_DATE"];
            }
            set
            {
                this["CREAT_DATE"] = value;
            }
        }
        public double? VALID_FLAG
        {
            get
            {
                return (double?)this["VALID_FLAG"];
            }
            set
            {
                this["VALID_FLAG"] = value;
            }
        }
        public string FAIL_DESC
        {
            get
            {
                return (string)this["FAIL_DESC"];
            }
            set
            {
                this["FAIL_DESC"] = value;
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
        public string WORKORDERNO
        {
            get
            {
                return (string)this["WORKORDERNO"];
            }
            set
            {
                this["WORKORDERNO"] = value;
            }
        }
        public string SN
        {
            get
            {
                return (string)this["SN"];
            }
            set
            {
                this["SN"] = value;
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
    }
    public class R_OUTLINE_TEST
    {
        public string DATA_TYPE { get; set; }
        public string LASTEDIT_BY { get; set; }
        public DateTime? LASTEDIT_DATE { get; set; }
        public string CREAT_EMP { get; set; }
        public DateTime? CREAT_DATE { get; set; }
        public double? VALID_FLAG { get; set; }
        public string FAIL_DESC { get; set; }
        public string STATUS { get; set; }
        public string LINE { get; set; }
        public string STATION { get; set; }
        public string SKUNO { get; set; }
        public string WORKORDERNO { get; set; }
        public string SN { get; set; }
        public string ID { get; set; }
    }
}