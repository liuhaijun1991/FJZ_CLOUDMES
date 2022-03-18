using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;

namespace MESDataObject.Module
{
    public class T_R_ALLPART_IC : DataObjectTable
    {
        public T_R_ALLPART_IC(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_R_ALLPART_IC(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_R_ALLPART_IC);
            TableName = "R_ALLPART_IC".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }

        //public DataTable GetQuackICList(OleExec DB, DB_TYPE_ENUM DBType,string Skuno)
        //{

        //    string StrSql = $@" SELECT Skuno, IC_PartNo, Location  FROM R_ALLPART_IC
        //   WHERE Skuno = '{Skuno}' OR Skuno IN
        //   (SELECT skuno73 FROM c_k_mapping WHERE skuno800 = '{Skuno}')
        //   UNION
        //   (SELECT  Skuno, IC_PartNo, Location FROM R_ALLPART_IC 
        //   WHERE Skuno IN(select b.kp_partno from c_kp_list a,c_kp_list_item b where a.skuno='{Skuno}' and a.id=b.list_id)
        //   )";
        //    DataTable Dt = null;
        //    try
        //    {
        //        Dt = DB.ExecSelect(StrSql).Tables[0];
        //    }
        //    catch(Exception ex)
        //    {
        //        throw ex;
        //    }
        //    return Dt;
        //}
    }
    public class Row_R_ALLPART_IC : DataObjectBase
    {
        public Row_R_ALLPART_IC(DataObjectInfo info) : base(info)
        {

        }
        public R_ALLPART_IC GetDataObject()
        {
            R_ALLPART_IC DataObject = new R_ALLPART_IC();
            DataObject.ID = this.ID;
            DataObject.SKUNO = this.SKUNO;
            DataObject.IC_PARTNO = this.IC_PARTNO;
            DataObject.LOCATION = this.LOCATION;
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
        public string IC_PARTNO
        {
            get
            {
                return (string)this["IC_PARTNO"];
            }
            set
            {
                this["IC_PARTNO"] = value;
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
    public class R_ALLPART_IC
    {
        public string ID { get; set; }
        public string SKUNO { get; set; }
        public string IC_PARTNO { get; set; }
        public string LOCATION { get; set; }
        public DateTime? EDIT_TIME { get; set; }
        public string EDIT_EMP { get; set; }
    }
}